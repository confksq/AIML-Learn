# Q&A — L33 IaC: Terraform for a Bicep Developer

*Created 2026-07-26 · FDE-Prep*

---

**Q1. One sentence: the biggest conceptual difference between Bicep and Terraform.**

Bicep lets Azure Resource Manager hold the deployment record; Terraform makes state an artifact
**you** own — which is why backends, locking, `import` and drift are Terraform vocabulary and not
Bicep vocabulary.

---

**Q2. Why must a production backend have locking? What provides it on AWS and Azure?**

Two concurrent `apply` runs writing the same state file corrupt it, and a corrupted state file can
mean Terraform no longer knows what it manages. AWS: a DynamoDB lock table. Azure: the blob lease,
provided automatically by the `azurerm` backend.

---

**Q3. Your secret resource has `sensitive = true`. Is the value safe in state?**

**No.** `sensitive = true` only suppresses it from CLI output. The value is stored in
`terraform.tfstate` in plaintext. Mitigate by encrypting the backend at rest, restricting read
access to the state store as tightly as a Key Vault, and never committing state.

---

**Q4. What does `-/+` mean in a plan, and what phrase do you search for before approving?**

Destroy **then** recreate. Search for `# forces replacement` — that is where unplanned outages come
from, because a changed name, AMI or location replaces rather than updates.

---

**Q5. Why prefer `for_each` over `count`?**

`count` indexes positionally, so removing a middle element renumbers everything after it and
Terraform destroys and recreates resources that did not change. `for_each` keys by a stable
identifier, so removing one affects only that one.

---

**Q6. `resource` vs `data`.**

`resource` = *I own this*; Terraform will create, update and destroy it. `data` = *I only read
this*; Terraform will never modify it. Use `data` for things another team or another state file owns.

---

**Q7. Why should CI apply a saved plan file rather than running a bare `terraform apply`?**

Because otherwise what a human approved and what gets applied can differ — state or reality may have
changed between the plan and the apply. `terraform plan -out=tfplan` then `terraform apply tfplan`
makes the approval binding.

---

**Q8. How do you decide state boundaries across an estate?**

By **blast radius and change frequency**. Networking changes rarely and breaks everything, so it
gets its own state. Platform (cluster, Key Vault) sits in another. Workloads that change daily sit
in a third. Cross-reference with `terraform_remote_state`.

---

**Q9. Why is Terraform the *native* choice on GCP rather than a third-party one?**

Google effectively stopped investing in its own IaC language — Deployment Manager is legacy and
**Infrastructure Manager is managed Terraform**. So Terraform is Google's own recommended path,
which means learning it covers AWS, GCP and Azure.

---

**Q10. Why is AWS CDK attractive given a .NET background? What does CloudFormation lack?**

CDK supports C#, so you get type safety, IDE completion and real xUnit tests, and helpers like
`bucket.GrantRead(fn)` generate correct least-privilege IAM instead of hand-written JSON.
CloudFormation has **no native loops** and no conditionals beyond a `Conditions` section — CDK gives
you plain `for` and `if`.

---

**Q11. Ansible vs Terraform — which layer does each own?**

Terraform (and Bicep, CloudFormation) **creates the box**. Ansible (and Puppet, Chef) **configures
inside the box**. In a containerised world the second layer largely disappears — the Dockerfile is
the config and Helm deploys it — so Ansible persists mainly for VMs, network gear and on-prem.

---

**Q12. Push vs pull, agent vs agentless: place Ansible and Puppet.**

Ansible: **agentless, push**, over SSH. Puppet: **agent-based, pull** — the agent checks in
periodically and enforces desired state continuously. Puppet suits large fleets needing continuous
enforcement; Ansible suits ad-hoc orchestration.

---

**Q13. A resource exists in AWS but not in state. Two ways to adopt it?**

The declarative `import { to = ..., id = ... }` block (Terraform 1.5+), optionally with
`terraform plan -generate-config-out=` to write the HCL for you; or the older CLI form
`terraform import <address> <id>`.

---

**Q14. Shift-left policy vs runtime policy — name a tool for each and why run both.**

Shift-left: Checkov or OPA/Conftest in the pipeline, failing the build before `apply`. Runtime:
AWS Config or Azure Policy, detecting and remediating after the fact. Both, because the pipeline only
governs what goes *through* the pipeline — anything created by hand or by another team needs the
runtime net.

---

**Q15. What does FedRAMP change about deploying an LLM application?**

The whole boundary must be authorised, not just your application — which in practice means Azure
Government or AWS GovCloud with a FedRAMP-authorised model endpoint, and rules out calling a public
model API directly. Prompt and completion logs become audit artifacts inside the boundary, so their
retention and access control are part of the authorisation.

---

**Q16. AWS PrivateLink — what problem does it solve that a firewall rule also solves, and why is it
better?**

Both let a private workload reach a service. A firewall rule permits egress to the public internet
for that destination; PrivateLink means the traffic **never leaves the private network** at all. Less
attack surface, no dependency on an egress rule staying correct, and no ticket to another team when
the destination changes.

---

## Scoring

| Score | Read |
|---|---|
| 14–16 | Rows 18–22, 24, 38, 39 are green. |
| 10–13 | Re-read §1 (state) and §9 (security/compliance) — the two densest sections. |
| < 10 | Re-read `L33`, then `08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` for the vocabulary. |
