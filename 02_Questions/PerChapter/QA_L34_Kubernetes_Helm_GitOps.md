# Q&A — L34 Kubernetes, Helm and GitOps

*Created 2026-07-26 · FDE-Prep*

---

**Q1. What problem does Helm solve that raw Kubernetes YAML cannot?**

Raw manifests have no variables, so every environment needs a near-identical copy that drifts apart
within weeks. Helm templates them and parameterises the differences — the same fix Bicep parameters
provide for ARM.

---

**Q2. Chart, Release, Revision — define each.**

Chart = the package (templates + default values). Release = a named, versioned installation of that
chart into a cluster. Revision = one version of a release; `helm rollback` targets revisions.

---

**Q3. What does `--atomic` do on `helm upgrade`, and why should it be a default?**

If the upgrade fails or exceeds `--timeout`, Helm automatically rolls back to the previous revision.
Without it a failed upgrade leaves you half-deployed — some pods new, some old, no clean state.

---

**Q4. Why does editing a ConfigMap not restart pods? Standard fix?**

Kubernetes only rolls pods when the **pod template** changes; a ConfigMap is a separate object.
Standard fix: hash the ConfigMap into a pod annotation
(`checksum/config: {{ ... | sha256sum }}`) so a config change alters the template and triggers a
rollout.

---

**Q5. Push vs pull deployment — the security advantage of pull?**

With push, CI holds cluster credentials — your build system has write access to production. With
pull, an in-cluster agent reads from Git and CI never touches the cluster at all. The credential
simply does not exist outside the cluster.

---

**Q6. What do `prune: true` and `selfHeal: true` do in an ArgoCD Application?**

`prune` deletes cluster resources that were removed from Git. `selfHeal` reverts manual changes back
to what Git says. Together they make Git genuinely authoritative rather than merely the starting
point.

---

**Q7. Someone runs `kubectl edit deployment` on a GitOps-managed cluster. What happens?**

With `selfHeal: true`, ArgoCD detects the divergence on its next sync (typically within a few
minutes) and reverts it. Without `selfHeal`, the Application shows as **OutOfSync** and waits — the
drift is visible but not corrected.

---

**Q8. AKS Workload Identity — the EKS equivalent, and how is it wired?**

**IRSA** — IAM Roles for Service Accounts. A Kubernetes ServiceAccount is annotated with an IAM role
ARN (`eks.amazonaws.com/role-arn`), and pods using that ServiceAccount receive scoped, short-lived
AWS credentials automatically. No secrets stored in the cluster.

---

**Q9. What does Karpenter do differently from the Cluster Autoscaler, and why does it matter for GPU
inference?**

Cluster Autoscaler scales pre-defined node groups. Karpenter provisions nodes **per pending pod**,
choosing the instance type that fits. For GPU inference that means getting exactly the accelerator a
workload requested — and using spot capacity where acceptable — instead of over-provisioning a fixed
GPU node group.

---

**Q10. Three things a service mesh gives you without code changes — and one case where you would not
use one.**

mTLS between all pods; retries, timeouts and circuit breaking at the network layer; traffic splitting
for canary releases. (Also: automatic golden-signal telemetry and authorization policy.) Would not
use one: fewer than roughly ten services, where the operational cost exceeds the benefit and a
resilience library plus ingress TLS and network policy covers it.

---

**Q11. Why do many public container images fail on OpenShift?**

OpenShift's Security Context Constraints block containers running as root and assign a random UID by
default. Images that assume root, or that write to paths owned by a fixed UID, fail. Charts usually
need `runAsNonRoot` and writable-path adjustments.

---

**Q12. What is an OIDC federated credential in a pipeline, and what problem does it remove?**

The workflow exchanges a short-lived identity token issued by the CI provider for a cloud access
token, based on a trust relationship configured in the cloud. It removes long-lived stored
secrets — there is no client secret to rotate, leak or expire.

---

**Q13. `helm template` vs `helm upgrade --dry-run` — when would you use each?**

`helm template` renders locally with no cluster contact — good in CI, and for diffing rendered output
between versions. `--dry-run` sends the request to the API server, so it also validates against the
live cluster's schema, admission controllers and existing state.

---

**Q14. Why is GitOps particularly valuable for AI workloads specifically?**

Because prompts, tool definitions, guardrail configuration and model routing are all just files. A
bad prompt becomes a `git revert` rather than a redeploy, model version pinning is a reviewed commit,
and an auditor asking who changed a guardrail and when gets `git log`. That is a compliance argument,
not just a convenience one.

---

**Q15. Name the AKS / EKS / GKE equivalents for: workload identity, secret injection, registry.**

| | AKS | EKS | GKE |
|---|---|---|---|
| Workload identity | Entra Workload Identity | IRSA | Workload Identity |
| Secret injection | Key Vault CSI driver | Secrets Manager CSI | Secret Manager CSI |
| Registry | ACR | ECR | Artifact Registry |

---

## Scoring

| Score | Read |
|---|---|
| 13–15 | Rows 28–33 are green. |
| 9–12 | Re-read §2 (chart anatomy) and §4 (GitOps). |
| < 9 | Re-read `L34`, then do the JM Family Anchor exercise on your own cluster — `helm list -A` first. |
