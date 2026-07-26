# IaC Terminology — Azure · AWS · GCP · Terraform

**Created:** 2026-07-26 · Part of **FDE-Prep** (see `FDE-Prep_Tracker.md`)
**Why this exists:** you know Bicep + Azure DevOps YAML. This file translates that knowledge into
AWS, GCP and Terraform vocabulary so the JD's IaC rows (#17–#22, #24) stop being unfamiliar words.
**Feeds:** `L33_IaC_Terraform_for_Bicep_Devs.md` (to be built)

---

## Part 1 — The five terms you asked about

### 1. DSL — "Domain-Specific Language"

A language built for **one job only**, as opposed to a general-purpose language.

| | Example | Can you write a web app in it? |
|---|---|---|
| **General-purpose language** | C#, Python, Java, Go | Yes — anything |
| **DSL** | **Bicep**, **HCL** (Terraform), SQL, Helm templates | **No.** Bicep only describes Azure resources |

So *"Bicep is a DSL"* means: Bicep exists purely to declare Azure infrastructure. It has no
`Main()`, no HTTP client, no ability to do anything except describe resources. That restriction is
the point — it stays small and readable.

**Bicep is not a runtime.** `.bicep` **transpiles** to ARM JSON, and ARM JSON is what Azure
actually executes. Bicep is a friendlier front-end over a format nobody enjoyed writing by hand.

```
main.bicep  ──(bicep build)──►  main.json (ARM)  ──►  Azure Resource Manager  ──►  resources
   you write this                 generated             executes it
```

### 2. Cross-cloud Terraform

Terraform is **one language and one CLI that talks to many clouds** via plug-ins called
**providers**. The workflow (`init` → `plan` → `apply`) is identical everywhere; only the provider
and the resource names change.

```hcl
provider "azurerm" {}                 provider "aws" {}
resource "azurerm_storage_account"    resource "aws_s3_bucket"
         "sa" { ... }                          "b" { ... }
```

Same file can contain both — one `terraform apply` provisions across Azure and AWS together.
That is what "cross-cloud" means. Bicep can never do this: it speaks only ARM.

> ⚠️ **Common misunderstanding.** Terraform is *not* a translation layer. `aws_s3_bucket` and
> `azurerm_storage_account` are different resources with different arguments. Terraform unifies the
> **workflow and language**, not the cloud APIs. You still have to know each cloud.

### 3. "CFN service" — CloudFormation

**CloudFormation (CFN)** is AWS's native IaC service — the direct equivalent of Azure Resource
Manager. You hand it a YAML/JSON template; it creates the resources, **remembers what it created**,
and can update or delete them as a set.

Calling it a *service* matters: it's a running AWS component with its own console, events and
error log — not just a file format.

### 4. `tfstate` — Terraform's memory

`terraform.tfstate` is a **JSON file mapping your code to real cloud resource IDs**.

```
your code:   resource "aws_s3_bucket" "logs" { ... }
                              ↕  tfstate remembers this pairing
real cloud:  arn:aws:s3:::jma-logs-20260726
```

Without it, Terraform cannot know that the bucket in your code *is* the bucket in AWS — it would
try to create a second one. This is the single biggest concept Bicep never made you learn, because
**Azure keeps that record for you inside ARM**.

Consequences you must handle with Terraform and never had to with Bicep:

| Concern | Why | Handling |
|---|---|---|
| **Storage** | Local file = lost laptop, lost infrastructure | Remote **backend** — Azure Blob, S3, GCS, Terraform Cloud |
| **Locking** | Two people applying at once corrupts it | State locking (DynamoDB table on AWS, blob lease on Azure) |
| **Secrets** | **State stores resource attributes in plaintext — including passwords and keys** | Encrypt at rest, restrict access, treat state as a secret |
| **Drift** | Someone changed things in the portal | `terraform plan` compares state vs reality and shows the delta |
| **Adoption** | Resource exists but isn't in state | `terraform import` / `import {}` block (1.5+) |

The plaintext-secrets point is a favourite interview question. Bicep devs are often unaware of it.

### 5. "Stack"

**AWS's unit of deployment** — a *named collection of resources created from one template and
managed as one unit*. Delete the stack, and every resource in it is deleted.

| Cloud | Equivalent concept |
|---|---|
| **AWS** | **Stack** (CloudFormation) |
| **Azure** | **Deployment**, scoped to a Resource Group / Subscription. *(Azure "Deployment Stacks" is a newer feature that adds true lifecycle grouping — closer to an AWS Stack)* |
| **GCP** | **Deployment** (Deployment Manager) |
| **Terraform** | **State / workspace** — the set of resources one state file tracks |
| **Pulumi** | **Stack** (borrowed the AWS term; also means environment: dev/prod) |

> ⚠️ "Stack" is overloaded. Pulumi uses it to mean *environment* (a dev stack, a prod stack). AWS
> uses it to mean *a deployed template instance*. Check the context.

---

## Part 2 — Master translation table

| Concept | **Azure** | **AWS** | **GCP** | **Terraform** |
|---|---|---|---|---|
| **Native IaC language** | Bicep (DSL) · ARM (JSON) | CloudFormation (YAML/JSON) | Deployment Manager (YAML + Jinja/Python) — *legacy* | HCL |
| **File extension** | `.bicep` → `.json` | `.yaml` / `.json` | `.yaml`, `.jinja`, `.py` | `.tf`, `.tfvars` |
| **Code-based IaC (real languages)** | — | **AWS CDK** — TS, Python, **C#**, Java, Go | Config Connector (K8s CRDs) | **Pulumi** · **CDKTF** |
| **The engine/service** | Azure Resource Manager (ARM) | CloudFormation (CFN) | Infrastructure Manager (managed Terraform) | Terraform CLI + providers |
| **Unit of deployment** | Deployment / Deployment Stack | **Stack** | Deployment | State / workspace |
| **Resource container** | **Resource Group** | *(no direct equivalent — tags + stacks)* | **Project** | — |
| **Where state lives** | **Azure (ARM)** | **AWS (CFN)** | **GCP** | **You** — `tfstate` in a backend |
| **Preview / dry run** | `az deployment group what-if` | **Change Set** | `--preview` | `terraform plan` |
| **Deploy** | `az deployment group create` | `aws cloudformation deploy` | `gcloud infra-manager deployments apply` | `terraform apply` |
| **Destroy** | `az group delete` / stack delete | `aws cloudformation delete-stack` | `... deployments delete` | `terraform destroy` |
| **Inputs** | `param` · `.bicepparam` | `Parameters:` | `properties` | `variable` · `.tfvars` |
| **Outputs** | `output` | `Outputs:` | `outputs` | `output` |
| **Reuse unit** | **Module** (`module` keyword) | **Nested stack** · CDK **Construct** | Template import | **Module** |
| **Public reuse library** | Azure Verified Modules · ACR registry | CDK **Construct Hub** | — | **Terraform Registry** *(the largest)* |
| **Loops** | `for` | ❌ **none natively** — needs CDK or macros | Jinja/Python loops | `count` · `for_each` |
| **Conditionals** | `if` | `Conditions:` section | Jinja/Python | `count = c ? 1 : 0` |
| **Secrets** | `@secure()` · Key Vault reference | `{{resolve:secretsmanager:…}}` · SSM | Secret Manager reference | `sensitive = true` — ⚠️ **still plaintext in state** |
| **Drift detection** | Azure Policy · `what-if` | **Drift detection** (first-class) | limited | `terraform plan` |
| **Import existing resource** | decompile from ARM export | Resource import / IMPORT change set | limited | `terraform import` · `import {}` block |
| **Lint / validate** | `bicep build` · ARM-TTK | `cfn-lint` | — | `terraform validate` · `tflint` |
| **Unit test** | Pester | **CDK assertions — real C# unit tests** | — | `terraform test` (1.6+) |
| **Integration test** | deploy to throwaway RG | deploy throwaway stack | throwaway project | **Terratest** (Go) |
| **Policy / guardrails** | **Azure Policy** | AWS Config · SCP · Guard | Org Policy | **Checkov** · **OPA/Conftest** · Sentinel |
| **CI/CD pipeline YAML** | Azure DevOps · GitHub Actions | CodePipeline · GitHub Actions | Cloud Build · GitHub Actions | any |

---

## Part 3 — Languages used in IaC

### By category

| Category | Tools | Language | Note |
|---|---|---|---|
| **Declarative DSL** | **Bicep** | Bicep | Azure-only, transpiles to ARM JSON |
| | **Terraform** | **HCL** (HashiCorp Configuration Language) | Multi-cloud. *The one to learn* |
| **Declarative markup** | ARM | JSON | Verbose — Bicep exists to escape it |
| | **CloudFormation** | **YAML** or JSON | YAML in practice |
| | Deployment Manager | YAML + Jinja2 / Python | GCP, legacy |
| | Kubernetes | YAML | manifests |
| **General-purpose code** | **AWS CDK** | TS, JS, Python, Java, **C#**, Go | Synthesizes CloudFormation |
| | **Pulumi** | TS, JS, Python, Go, **C#**, Java, YAML | Own state model, multi-cloud |
| | **CDKTF** | TS, Python, **C#**, Java, Go | CDK syntax → Terraform |
| **Config-as-code** | **Ansible** | **YAML** (+ Jinja2) | Agentless, SSH. *Configures* servers |
| | **Puppet** | Puppet DSL (Ruby-like) | Agent-based, pull model |
| | Chef | Ruby DSL | Agent-based |
| | Salt | YAML + Jinja | — |
| **K8s packaging** | **Helm** | **Go templates + YAML** | Charts, values, releases |
| | Kustomize | YAML | Overlays, no templating |
| **Policy-as-code** | **OPA / Conftest** | **Rego** | Open standard |
| | Sentinel | Sentinel | Terraform Enterprise |
| | **Checkov** | YAML/Python rules | Scans Terraform, CFN, Bicep, K8s |
| **Testing** | Terratest | **Go** | Real deploy + assert |
| | Pester | PowerShell | Azure/Bicep |
| | CDK assertions | **C#** / TS / Python | Unit-testable infrastructure |

### Declarative vs imperative — the split that matters

```
DECLARATIVE                          IMPERATIVE
"I want 3 VMs."                      "Create a VM. Create a VM. Create a VM."

Bicep, ARM, CloudFormation,          Bash + az CLI, PowerShell,
Terraform HCL, K8s YAML              boto3, raw SDK scripts

Tool computes the diff and           You compute the diff. Run twice →
converges. Run twice → same          possibly 6 VMs.
result. IDEMPOTENT.

✅ IaC                                ❌ scripting, not IaC
```

**AWS CDK and Pulumi look imperative but are not.** You write imperative-looking C#/TypeScript, but
it *generates* a declarative template which the engine then applies. Best of both: loops, types,
IDE completion, unit tests — with declarative convergence.

### Config-as-code ≠ Infrastructure-as-code

The JD lists both, and they are different layers:

```
IaC (Terraform, Bicep, CFN)      →  CREATES the box
                                     "make me a VM, a VNet, a Key Vault"

Config-as-code (Ansible, Puppet) →  CONFIGURES inside the box
                                     "install nginx, write this config, start the service"
```

In a container world the second layer largely disappears — the Dockerfile *is* the config, and Helm
deploys it. Ansible and Puppet persist for VMs, network gear and on-prem.

---

## Part 4 — Universal IaC vocabulary

| Term | Meaning |
|---|---|
| **Declarative** | Describe the desired end state; the tool works out the steps |
| **Idempotent** | Running it twice changes nothing the second time. The core IaC property |
| **Desired state** | What your code says should exist |
| **Drift** | Reality diverged from desired state — usually someone edited the portal |
| **Convergence** | The tool changing reality until it matches desired state |
| **Plan / preview / change set / what-if** | Show me what *would* change before it does |
| **Apply** | Actually make the change |
| **State** | The record of what was deployed. Cloud-managed (Bicep, CFN) or self-managed (Terraform) |
| **Provider / plug-in** | The adapter Terraform uses to speak to one cloud's API |
| **Resource** | One thing being managed — a bucket, a VNet, a cluster |
| **Module / construct / nested stack** | A reusable, parameterised bundle of resources |
| **Registry** | A public library of shareable modules |
| **Backend** | Where Terraform stores state remotely |
| **State lock** | Prevents two people applying at once |
| **Immutable infrastructure** | Never patch in place — replace with a new version |
| **GitOps** | Git is the source of truth; an agent (**ArgoCD**, Flux) continuously reconciles the cluster to match the repo |
| **Blast radius** | How much breaks if this change is wrong. Drives how you split state files |

---

## Part 5 — What this means for you specifically

| You already know | Maps to | Delta to learn |
|---|---|---|
| **Bicep** | Terraform HCL, CloudFormation | **State ownership** — the only genuinely new concept |
| **ARM deployments** | CFN Stacks, TF workspaces | naming only |
| **`az deployment ... what-if`** | Change Set, `terraform plan` | naming only |
| **Azure DevOps YAML** | GitHub Actions, Cloud Build | syntax swap — **row #26 is not a real gap** |
| **Bicep modules** | TF modules, CDK constructs, nested stacks | naming only |
| **C# / .NET** | **AWS CDK in C#**, Pulumi in C# | ⭐ **your unfair advantage** |

### Three things to say in an interview

**On Terraform, coming from Bicep:**
> "Same model — declarative desired state, a preview step, idempotent apply. The real difference is
> state ownership. Bicep lets Azure Resource Manager hold the deployment record; Terraform makes
> state an artifact I own, which means a remote backend, locking, and treating the state file as a
> secret because resource attributes land in it in plaintext. That's the delta I'd be learning —
> not the concept of IaC."

**On AWS, coming from .NET:**
> "I'd reach for CDK on AWS — I can write infrastructure in C#, get type safety and real unit tests,
> and it synthesizes to CloudFormation. For anything multi-cloud, Terraform, because GCP has
> effectively standardised on it too — Infrastructure Manager is managed Terraform."

**On guardrails (the JD's word):**
> "`terraform plan` plus Checkov or OPA in the pipeline, failing the build on a policy violation
> before apply — so a public S3 bucket never reaches an account. On Azure the same job is Azure
> Policy at the subscription scope."

---

## Status log

| Date | Event |
|---|---|
| 2026-07-26 | Created in response to terminology questions (DSL, cross-cloud, CFN service, tfstate, stack). Feeds `L33`. |
