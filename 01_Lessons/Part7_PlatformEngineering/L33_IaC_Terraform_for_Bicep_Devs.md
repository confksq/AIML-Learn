# Module 33 — Infrastructure as Code: Terraform for a Bicep Developer

**Part 7: Platform Engineering & AI-Assisted Delivery**
*Created: 2026-07-26 · FDE-Prep · Clears tracker rows 18, 19, 20, 21, 22, 24, 38, 39*

> **Companion glossary:** `08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` — read it first if
> "stack", "tfstate", "CFN" or "DSL" are still fuzzy. This module assumes those terms.

---

## Why This Module Exists

You already do IaC. You write Bicep, you deploy through Azure DevOps YAML pipelines, you understand
declarative desired state and idempotent apply. **The concept is not new to you.**

So this module does not teach IaC. It teaches the **one thing Bicep never made you learn** — state
ownership — and then translates your existing vocabulary into Terraform, AWS and GCP.

| What you have | What's genuinely new |
|---|---|
| Declarative desired state ✅ | **You own the state file** |
| Idempotent apply ✅ | Remote backends and locking |
| Preview before deploy (`what-if`) ✅ | `terraform import` for existing resources |
| Modules and parameters ✅ | Providers, and multi-cloud in one file |
| Pipeline integration ✅ | Policy-as-code gates (Checkov / OPA) |

Budget: **~3 hours**, not the 8 a beginner needs. The JD says *"AWS preferred"*, so every example
leads with AWS and shows the Azure equivalent second.

---

## Section 1 — The Delta: Who Owns State

### 1.1 The picture

```
BICEP / ARM                              TERRAFORM
───────────                              ─────────
main.bicep                               main.tf
    │                                        │
    ▼                                        ▼
Azure Resource Manager                   Terraform CLI
    │                                        │
    ├── executes                             ├── reads terraform.tfstate  ◄── YOU own this
    └── REMEMBERS the deployment             ├── compares to real cloud
        (deployment history is a             ├── computes a diff
         first-class Azure resource)         └── applies the diff

Lose your laptop → nothing lost.         Lose the state → Terraform believes nothing
Azure still knows.                       exists and tries to create it all again.
```

### 1.2 What state ownership forces you to handle

| Concern | Bicep | Terraform |
|---|---|---|
| Where is the record? | Azure keeps it | **A file you must store** |
| Two people deploy at once | ARM serialises | **You configure locking or corrupt it** |
| Secrets in the record | Not exposed | ⚠️ **Plaintext in state** |
| Resource created by hand | Bicep just adopts it on next deploy | **`terraform import` or it fights you** |
| Someone edited the portal | `what-if` shows a diff | `plan` shows drift |

### 1.3 The remote backend — always configure one

Local state is a single-developer toy. Production always uses a remote backend with locking.

```hcl
# AWS — S3 + DynamoDB lock table
terraform {
  backend "s3" {
    bucket         = "jma-tfstate-prod"
    key            = "platform/ai/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "jma-tfstate-locks"     # ← the lock
    encrypt        = true
  }
}
```

```hcl
# Azure — blob container (lease provides the lock automatically)
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-tfstate"
    storage_account_name = "jmatfstate"
    container_name       = "tfstate"
    key                  = "platform/ai.tfstate"
  }
}
```

### 1.4 ⚠️ State contains secrets in plaintext

```hcl
resource "azurerm_key_vault_secret" "api" {
  name         = "openai-key"
  value        = var.openai_key
  key_vault_id = azurerm_key_vault.kv.id
}
```

The value lands in `terraform.tfstate` **in clear text**, even though the resource itself is a
secret store. `sensitive = true` only hides it from CLI output — it does nothing to the state file.

**Therefore:** encrypt the backend at rest, restrict read access to the state bucket/container as
tightly as you would a Key Vault, and never commit state to Git.

Bicep developers routinely do not know this. It is a favourite interview question.

---

## Section 2 — HCL for a Bicep Developer

Side by side. Same storage account, both languages.

```bicep
// Bicep
param location string = resourceGroup().location
param saName string

resource sa 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: saName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
}

output saId string = sa.id
```

```hcl
# Terraform — Azure
variable "location" { type = string, default = "eastus" }
variable "sa_name"  { type = string }

resource "azurerm_storage_account" "sa" {
  name                     = var.sa_name
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

output "sa_id" { value = azurerm_storage_account.sa.id }
```

```hcl
# Terraform — AWS equivalent
resource "aws_s3_bucket" "logs" {
  bucket = var.bucket_name
  tags   = { Environment = "prod", Owner = "ai-platform" }
}
```

### 2.1 Syntax translation table

| Bicep | Terraform HCL |
|---|---|
| `param x string` | `variable "x" { type = string }` |
| `var y = ...` | `locals { y = ... }` |
| `output z string = ...` | `output "z" { value = ... }` |
| `resource sa '...' = { }` | `resource "azurerm_storage_account" "sa" { }` |
| `sa.id` | `azurerm_storage_account.sa.id` |
| `module m 'x.bicep' = { }` | `module "m" { source = "./x" }` |
| `[for i in range(0,3): ...]` | `count = 3` or `for_each = toset([...])` |
| `if (cond)` | `count = cond ? 1 : 0` |
| `@secure()` | `sensitive = true` ⚠️ *(state still plaintext)* |
| `resourceGroup().location` | `data "azurerm_resource_group" "rg" {}` |

### 2.2 `count` vs `for_each` — get this right

```hcl
count    = 3                                  # indexed: aws_instance.web[0..2]
for_each = toset(["dev", "test", "prod"])     # keyed:   aws_instance.web["dev"]
```

**Prefer `for_each`.** With `count`, removing the middle item renumbers everything after it, and
Terraform destroys and recreates resources that did not change. With `for_each`, keys are stable.

### 2.3 Data sources — read something you did not create

```hcl
data "azurerm_key_vault" "existing" {
  name                = "kv-jma-prod"
  resource_group_name = "rg-shared"
}

resource "azurerm_key_vault_secret" "s" {
  key_vault_id = data.azurerm_key_vault.existing.id   # reference, don't manage
  ...
}
```

`resource` = *I own this*. `data` = *I only read this*. Terraform will never modify a data source.

---

## Section 3 — The Workflow

```bash
terraform init        # download providers, configure backend       (once per checkout)
terraform fmt         # canonical formatting
terraform validate    # syntax + type check, no cloud calls
terraform plan        # ← the important one. Shows the diff. Read it.
terraform apply       # execute
terraform destroy     # tear down everything in this state
```

### 3.1 Reading a plan

```
  # aws_s3_bucket.logs will be created
  + resource "aws_s3_bucket" "logs" {
      + bucket = "jma-logs-prod"
    }

  # aws_instance.web must be replaced
-/+ resource "aws_instance" "web" {
      ~ ami = "ami-old" -> "ami-new" # forces replacement
    }

Plan: 2 to add, 0 to change, 1 to destroy.
```

| Symbol | Meaning |
|---|---|
| `+` | create |
| `-` | destroy |
| `~` | update in place |
| `-/+` | **destroy then recreate** — ⚠️ read carefully, this is where outages come from |

**`# forces replacement`** is the phrase to search for in every plan before approving. A changed
`ami`, `name`, or `location` on many resources means destroy-and-recreate, not an in-place edit.

### 3.2 In a pipeline

```yaml
# Azure DevOps — you already know this shape
- script: terraform init
- script: terraform validate
- script: checkov -d . --framework terraform      # ← policy gate, fails the build
- script: terraform plan -out=tfplan
- task: ManualValidation@0                        # human approves the plan
- script: terraform apply tfplan                  # apply the SAVED plan, not a fresh one
```

**Always `apply` a saved plan file**, never a bare `terraform apply` in CI. Otherwise what a human
approved and what gets applied can differ.

---

## Section 4 — Modules and Reuse

```hcl
module "ai_platform" {
  source  = "./modules/ai-platform"        # local
  # source = "terraform-aws-modules/vpc/aws"      # public registry
  # source = "git::https://github.com/jma/tf-modules.git//ai?ref=v1.2.0"

  environment  = "prod"
  model_region = "us-east-1"
}

output "endpoint" { value = module.ai_platform.endpoint }
```

Identical concept to a Bicep module. The difference is the **Terraform Registry** — the largest
public IaC module library in existence. `terraform-aws-modules/vpc/aws` gives you a
production-grade, peer-reviewed VPC in six lines. Azure's nearest equivalent is Azure Verified
Modules, which is far smaller.

**Blast radius and state splitting.** Do not put an entire estate in one state file. Split by
lifecycle and risk:

```
state/networking/     ← changes rarely, breaks everything
state/platform/       ← AKS/EKS, Key Vault
state/ai-workloads/   ← changes daily
```

Then reference across with `terraform_remote_state` data sources. "How do you decide state
boundaries?" is a senior interview question; the answer is **blast radius**.

---

## Section 5 — Multi-Cloud in One File

```hcl
terraform {
  required_providers {
    aws     = { source = "hashicorp/aws",     version = "~> 5.0" }
    azurerm = { source = "hashicorp/azurerm", version = "~> 3.0" }
  }
}

provider "aws"     { region = "us-east-1" }
provider "azurerm" { features {} }

resource "aws_s3_bucket" "raw"            { bucket = "jma-raw" }
resource "azurerm_storage_account" "curated" { ... }
```

One `apply`, both clouds. **This is what "cross-cloud" means** — and it is impossible in Bicep or
CloudFormation.

> ⚠️ Terraform unifies the *workflow and language*, not the cloud APIs. `aws_s3_bucket` and
> `azurerm_storage_account` have entirely different arguments. You still have to know each cloud.

### 5.1 GCP — Terraform *is* the native approach

Google effectively stopped investing in its own IaC language. Deployment Manager is legacy;
**Infrastructure Manager is managed Terraform**. So learning Terraform covers GCP by default.

```hcl
provider "google" { project = "jma-ai-prod", region = "us-central1" }
resource "google_storage_bucket" "raw" { name = "jma-raw", location = "US" }
```

**This is the single best argument for spending your IaC hours on Terraform rather than
CloudFormation:** it covers AWS, GCP *and* Azure.

---

## Section 6 — AWS CDK: Your Unfair Advantage

CDK lets you write infrastructure in **C#**, then synthesizes CloudFormation.

```csharp
using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.Lambda;

public class AiPlatformStack : Stack
{
    public AiPlatformStack(Construct scope, string id, IStackProps props = null)
        : base(scope, id, props)
    {
        var raw = new Bucket(this, "RawDocs", new BucketProps {
            Versioned  = true,
            Encryption = BucketEncryption.S3_MANAGED
        });

        var fn = new Function(this, "Extractor", new FunctionProps {
            Runtime = Runtime.PYTHON_3_12,
            Handler = "app.handler",
            Code    = Code.FromAsset("src")
        });

        raw.GrantRead(fn);          // ← generates the whole IAM policy for you
    }
}
```

```bash
cdk synth     # → CloudFormation template
cdk diff      # ← the change-set preview
cdk deploy
```

| Why this matters for you | |
|---|---|
| Language | **C#** — your primary language |
| Type safety | compile-time errors, IDE completion |
| Testing | **real xUnit tests** via CDK assertions |
| Loops/conditionals | plain C# `for` and `if` — CloudFormation has **no native loops** |
| IAM | `GrantRead()` writes correct least-privilege policy so you don't hand-craft JSON |

**Interview line:**
> "On AWS I'd reach for CDK — I write infrastructure in C# with type safety and unit tests, and it
> synthesizes to CloudFormation, so operationally it's still a standard CFN stack. For anything
> multi-cloud I'd use Terraform, since GCP has effectively standardised on it too."

---

## Section 7 — The Rest of the Landscape

### 7.1 Pulumi

Same idea as CDK but **not** tied to one cloud, and with its own state model (Pulumi Cloud or
self-managed). Languages: TS, Python, Go, **C#**, Java.

| | CDK | Pulumi | Terraform |
|---|---|---|---|
| Clouds | AWS only | any | any |
| Language | real code | real code | HCL |
| State | CloudFormation (AWS holds) | Pulumi backend (you hold) | `tfstate` (you hold) |

**When to pick Pulumi:** a strong .NET/TS team that wants real code *and* multi-cloud. Its downside
is a much smaller module ecosystem than the Terraform Registry.

### 7.2 Ansible and Puppet — a different layer

```
IaC (Terraform / Bicep / CFN)   →  CREATES the box
Config-as-code (Ansible/Puppet) →  CONFIGURES inside the box
```

```yaml
# Ansible — YAML, agentless, over SSH. Push model.
- name: Configure inference host
  hosts: gpu_nodes
  tasks:
    - name: Install NVIDIA container toolkit
      ansible.builtin.package:
        name: nvidia-container-toolkit
        state: present
    - name: Ensure ollama service running
      ansible.builtin.systemd:
        name: ollama
        state: started
        enabled: true
```

| | Ansible | Puppet |
|---|---|---|
| Language | **YAML** + Jinja2 | Puppet DSL (Ruby-like) |
| Agent | **none** — SSH | agent on every node |
| Model | **push** | **pull** — agent checks in every 30 min |
| Best for | ad-hoc config, orchestration, network gear | large fleets needing continuous enforcement |

**In a container world this layer mostly disappears** — the Dockerfile is the config and Helm
deploys it (`L34`). Ansible and Puppet persist for VMs, network devices and on-prem estates. Say
that in an interview; it shows you know when *not* to use a tool.

### 7.3 VMware — awareness only

On-prem virtualisation. Terraform has a `vsphere` provider, so VMs, networks and datastores can be
managed as code. Relevant because enterprise infrastructure orgs still run large vSphere estates
and "modernization" often means *lift from vSphere to cloud*. You do not need depth here — you need
to not blink when it is mentioned.

---

## Section 8 — Cloud Migration

The JD lists it under Preferred Skills, and the account already claims a **60% reduction in
migration effort**.

### 8.1 The 6 Rs

| Strategy | Meaning | IaC involvement |
|---|---|---|
| **Rehost** | lift and shift, VM → VM | low — provision target VMs |
| **Replatform** | lift and reshape, e.g. SQL Server → managed SQL | medium |
| **Refactor** | rewrite for cloud-native | **high** — all-new IaC |
| **Repurchase** | move to SaaS | none |
| **Retire** | switch it off | none |
| **Retain** | leave it | none |

### 8.2 Where AI reduces migration effort — the JD's actual claim

| Manual task | AI-assisted |
|---|---|
| Read 400 VMs' configs, hand-write Terraform | LLM generates modules from discovery output |
| Translate ARM/CFN → Terraform | LLM does the first pass; `plan` verifies it |
| Map legacy dependencies | LLM reads config + code, produces a dependency graph |
| Write runbooks | generated from the IaC itself |

**The verification point matters more than the generation point.** `terraform plan` is a
deterministic check on non-deterministic output — the LLM writes it, the plan proves it. That is
the answer to *"how do you trust AI-generated infrastructure?"*

### 8.3 `terraform import` — adopting what already exists

Migration always means resources that exist but are not in state.

```hcl
import {                                  # Terraform 1.5+ declarative form
  to = aws_s3_bucket.legacy
  id = "jma-legacy-docs"
}
```

```bash
terraform plan -generate-config-out=generated.tf   # writes the HCL for you
```

Older CLI form: `terraform import aws_s3_bucket.legacy jma-legacy-docs`.

---

## Section 9 — Cloud Security and Compliance in IaC

### 9.1 VPC and PrivateLink (JD row 39)

| Concept | AWS | Azure equivalent |
|---|---|---|
| Private network | **VPC** | VNet |
| Subnet | Subnet | Subnet |
| Private service access | **PrivateLink / VPC Endpoint** | **Private Endpoint** |
| Firewall rules | Security Group + NACL | NSG |
| Outbound control | NAT Gateway | Azure Firewall / NAT |

```hcl
# Keep Bedrock traffic off the public internet entirely
resource "aws_vpc_endpoint" "bedrock" {
  vpc_id             = aws_vpc.main.id
  service_name       = "com.amazonaws.us-east-1.bedrock-runtime"
  vpc_endpoint_type  = "Interface"
  subnet_ids         = aws_subnet.private[*].id
  security_group_ids = [aws_security_group.endpoints.id]
}
```

**This is the same problem you solved on the CallMiner pipeline** — AKS egress blocked by the hub
firewall until a rule was added. PrivateLink is the AWS way of never needing that rule, because the
traffic never leaves the private network.

### 9.2 Compliance for LLM workloads (JD row 38)

| Framework | What it is | LLM-specific concern |
|---|---|---|
| **SOC 2** | Trust-services audit (security, availability, confidentiality) | Vendor attestation; is your model provider SOC 2 Type II? |
| **HIPAA** | US health data | BAA with the model provider; no PHI in prompts/logs without one |
| **FedRAMP** | US federal cloud authorisation | Requires a FedRAMP-authorised boundary. **Azure OpenAI in Azure Government** and **Bedrock in GovCloud** are the usual answers |

**FedRAMP for LLM — the one-liner:**
> "FedRAMP means the whole boundary has to be authorised, not just the app. In practice that pushes
> you to Azure Government or AWS GovCloud with a FedRAMP-authorised model endpoint, and it rules out
> calling a public model API directly. Prompt and completion logging becomes an audit control, so
> retention and access on those logs is part of the boundary too."

### 9.3 Policy-as-code — the JD's "guardrails"

```bash
checkov -d . --framework terraform        # scans Terraform, CFN, Bicep, K8s, Dockerfiles
```

```
FAILED  CKV_AWS_18: "Ensure the S3 bucket has access logging enabled"
FAILED  CKV_AWS_21: "Ensure the S3 bucket has versioning enabled"
```

Wire it before `plan` in the pipeline and fail the build. The result: a public S3 bucket cannot
reach an account, because the pipeline rejects it at commit time rather than an auditor finding it
in six months.

| Tool | Language | Scope |
|---|---|---|
| **Checkov** | Python rules | Terraform, CFN, Bicep, K8s — easiest to adopt |
| **OPA / Conftest** | **Rego** | Anything JSON/YAML — the open standard |
| **Sentinel** | Sentinel | Terraform Enterprise only |
| **Azure Policy** | JSON | Azure, enforced at subscription scope after deploy |

**The distinction worth stating:** Checkov/OPA are *shift-left* — they block before deploy. Azure
Policy and AWS Config are *runtime* — they detect and remediate after. Mature platforms run both.

---

## Section 10 — Testing IaC

| Level | Terraform | Azure/Bicep | AWS CDK |
|---|---|---|---|
| Format | `terraform fmt` | `bicep format` | `dotnet format` |
| Lint | `tflint` | ARM-TTK | analyzers |
| Validate | `terraform validate` | `bicep build` | `cdk synth` |
| Policy | **Checkov / OPA** | Checkov / Azure Policy | Checkov / cdk-nag |
| Dry run | **`terraform plan`** | `az deployment group what-if` | **`cdk diff`** |
| Unit test | `terraform test` (1.6+) | Pester | **xUnit + CDK assertions (C#)** |
| Integration | **Terratest** (Go) | deploy to throwaway RG | deploy throwaway stack |

```hcl
# tests/storage.tftest.hcl — native Terraform testing
run "bucket_is_private" {
  command = plan
  assert {
    condition     = aws_s3_bucket_public_access_block.this.block_public_acls == true
    error_message = "Bucket must block public ACLs"
  }
}
```

---

## JM Family Anchor

| Your real work | The Terraform version |
|---|---|
| AKS cluster + node pools provisioned by hand/Bicep | `azurerm_kubernetes_cluster` + `azurerm_kubernetes_cluster_node_pool` |
| Key Vault + secret for the DSX scan client | `azurerm_key_vault` + `azurerm_key_vault_secret` ⚠️ *value lands in state* |
| Firewall rule for AKS → CallMiner SFTP egress | `azurerm_firewall_network_rule_collection` — reviewable in a PR instead of a ticket |
| Postgres access to Alvaria RAS | `azurerm_private_endpoint` — no firewall exception needed |
| The RBAC Role/RoleBinding you could not apply (blocked on another team) | `kubernetes_role` + `kubernetes_role_binding`, versioned and PR-reviewed |

That last row is the strongest argument for IaC you personally have: a ticket that sat blocked for
days becomes a reviewed pull request.

---

## Self-Test Questions

1. What is the single biggest conceptual difference between Bicep and Terraform?
2. Why must a production backend have locking, and what provides it on AWS and on Azure?
3. Your Key Vault secret is `sensitive = true`. Is the value safe in the state file? Explain.
4. What does `-/+` mean in a plan, and which phrase do you search for before approving?
5. Why prefer `for_each` over `count`?
6. Difference between `resource` and `data`?
7. Why should a pipeline apply a *saved* plan file rather than running `terraform apply` fresh?
8. How would you split state files across an estate, and on what principle?
9. Terraform on GCP — why is it the *native* choice rather than a third-party one?
10. Why is AWS CDK particularly attractive given your background? What does CloudFormation lack that
    CDK provides?
11. Ansible vs Terraform — which layer does each own?
12. A resource exists in AWS but not in state. Two ways to adopt it?
13. Shift-left policy vs runtime policy — name a tool for each and when you would run both.
14. What does FedRAMP change about how you deploy an LLM application?

---

## Quick-Reference Interview Answers

**"You're a Bicep person. We use Terraform."**
> "Same model — declarative desired state, a preview step, idempotent apply, modules for reuse. The
> real difference is state ownership. Bicep lets Azure Resource Manager hold the deployment record;
> Terraform makes state an artifact I own. That means a remote backend with locking, splitting state
> by blast radius, `terraform import` to adopt existing resources, and treating the state file as a
> secret because resource attributes land in it in plaintext. The HCL syntax took me an afternoon —
> state was the concept worth learning."

**"How do you stop someone shipping a public S3 bucket?"**
> "Checkov or OPA in the pipeline before `plan`, failing the build on a policy violation, so it never
> reaches an account. That's shift-left. I'd pair it with AWS Config or Azure Policy at runtime for
> anything created outside the pipeline. Both, because the pipeline only governs what goes through
> the pipeline."

**"How do you trust AI-generated infrastructure code?"**
> "Because `terraform plan` is a deterministic check on non-deterministic output. The model writes
> the module, the plan tells me exactly what will be created, changed or destroyed, and Checkov tells
> me whether it violates policy. I read the plan for `forces replacement` before approving. That's how
> you get large migration-effort reductions safely — AI does the generation, the toolchain does the
> verification."

**"Multi-cloud strategy?"**
> "Terraform for anything spanning clouds — one language, one workflow, and it's effectively GCP's
> native path since Infrastructure Manager is managed Terraform. Bicep stays where a team is
> Azure-only and wants the tightest Azure integration. On AWS specifically I'd consider CDK in C#,
> because I get type safety and real unit tests and it still deploys as a standard CloudFormation
> stack. What I wouldn't do is claim Terraform makes clouds interchangeable — it unifies the
> workflow, not the APIs."

---

## Related

`08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` (terminology) · `L34` (Helm/GitOps — the layer above) ·
`L19` (CI/CD pipelines) · `L36` (observability of what you deployed) ·
`VitalCare_AI_Assessment_Response.md:447, :1540–1560` (cloud-agnostic stack decisions)
