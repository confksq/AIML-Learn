# Module 34 — Kubernetes, Helm and GitOps for AI Platforms

**Part 7: Platform Engineering & AI-Assisted Delivery**
*Created: 2026-07-26 · FDE-Prep · Clears tracker rows 28, 29, 30, 31, 32, 33*

> **You already run AKS** — kubectl, PIM activation, diagnosing `CrashLoopBackOff`, Key Vault
> integration, egress through a hub firewall. This module does not teach Kubernetes. It teaches the
> **packaging and delivery layer above it** — Helm and GitOps — plus the AWS/GCP/OpenShift
> equivalents you would meet on this JD.

---

## Why This Module Exists

`L20:39–40` covers AKS in **two lines**. `L31` mentions pod restarts in a monitoring table. That is
the entire Kubernetes content of this library — because the curriculum targeted AI-102, where
Kubernetes is a deployment target, not a subject.

The JD asks for **Helm, service mesh, ArgoCD, EKS/GKE/AKS, OpenShift**. Here they are.

| You have | You need |
|---|---|
| kubectl, pods, deployments, namespaces ✅ | **Helm** — templating and release management |
| YAML fluency ✅ | **ArgoCD** — continuous reconciliation |
| Azure DevOps push-based deploys ✅ | **Pull-based GitOps** — a different model |
| AKS ✅ | EKS / GKE / OpenShift differences |

Budget: **~3 hours**, because YAML and pipelines are already yours.

---

## Section 1 — Where Helm Fits

```
Dockerfile        →  builds the image
Kubernetes YAML   →  describes ONE deployment, hardcoded
Helm chart        →  TEMPLATES that YAML, parameterised per environment
ArgoCD            →  keeps the cluster matching Git, continuously
```

### 1.1 The problem Helm solves

Raw Kubernetes YAML has no variables. Three environments means three near-identical copies:

```
k8s/dev/deployment.yaml     replicas: 1   image: agent:dev    cpu: 250m
k8s/test/deployment.yaml    replicas: 2   image: agent:test   cpu: 500m
k8s/prod/deployment.yaml    replicas: 6   image: agent:1.4.2  cpu: 2000m
                            ↑ 95% identical, drifts apart within a month
```

**This is exactly the problem Bicep parameters solve for ARM.** Helm is the same fix for Kubernetes.

---

## Section 2 — Chart Anatomy

```
cancellation-agent/
├── Chart.yaml            # name, version, appVersion — the manifest
├── values.yaml           # DEFAULT parameter values
├── values-prod.yaml      # per-environment overrides
├── templates/
│   ├── deployment.yaml   # Go-templated K8s manifests
│   ├── service.yaml
│   ├── ingress.yaml
│   ├── hpa.yaml
│   └── _helpers.tpl      # reusable template functions
└── charts/               # vendored sub-charts (dependencies)
```

**`Chart.yaml`**

```yaml
apiVersion: v2
name: cancellation-agent
version: 0.3.1          # the CHART version — bump on template changes
appVersion: "1.4.2"     # the APP version — the container image tag
```

**`values.yaml` — the parameters file**

```yaml
replicaCount: 2

image:
  repository: jmaacr.azurecr.io/cancellation-agent
  tag: ""                       # empty → falls back to Chart appVersion
  pullPolicy: IfNotPresent

resources:
  requests: { cpu: 250m, memory: 512Mi }
  limits:   { cpu: 1000m, memory: 2Gi }

env:
  AZURE_OPENAI_ENDPOINT: https://jma-openai.openai.azure.com/
  MODEL_DEPLOYMENT: gpt-4o

keyVault:
  enabled: true
  name: kv-jma-ai

autoscaling:
  enabled: true
  minReplicas: 2
  maxReplicas: 10
  targetCPUUtilizationPercentage: 70
```

**`templates/deployment.yaml`**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ include "cancellation-agent.fullname" . }}
  labels: {{- include "cancellation-agent.labels" . | nindent 4 }}
spec:
  {{- if not .Values.autoscaling.enabled }}
  replicas: {{ .Values.replicaCount }}
  {{- end }}
  selector:
    matchLabels: {{- include "cancellation-agent.selectorLabels" . | nindent 6 }}
  template:
    metadata:
      annotations:
        checksum/config: {{ include (print $.Template.BasePath "/configmap.yaml") . | sha256sum }}
    spec:
      containers:
        - name: agent
          image: "{{ .Values.image.repository }}:{{ .Values.image.tag | default .Chart.AppVersion }}"
          imagePullPolicy: {{ .Values.image.pullPolicy }}
          resources: {{- toYaml .Values.resources | nindent 12 }}
          env:
            {{- range $k, $v := .Values.env }}
            - name: {{ $k }}
              value: {{ $v | quote }}
            {{- end }}
          readinessProbe:
            httpGet: { path: /healthz, port: 8080 }
            initialDelaySeconds: 10
          livenessProbe:
            httpGet: { path: /healthz, port: 8080 }
            initialDelaySeconds: 30
```

### 2.1 Template syntax you must recognise

| Syntax | Means |
|---|---|
| `{{ .Values.x }}` | value from `values.yaml` |
| `{{ .Chart.Name }}` | from `Chart.yaml` |
| `{{ .Release.Name }}` | the install name — set at `helm install` time |
| `{{- ... }}` / `{{ ... -}}` | trim whitespace **left / right** — YAML is indentation-sensitive, this matters |
| `\| nindent 4` | indent by 4 with a leading newline |
| `\| quote` | wrap in quotes |
| `\| default X` | fallback |
| `{{- if }}` `{{- range }}` | conditionals and loops |
| `include "name" .` | call a named template from `_helpers.tpl` |

> ⚠️ **The `checksum/config` annotation** in the example is a real-world trick: changing a ConfigMap
> does **not** restart pods on its own. Hashing the ConfigMap into a pod annotation forces a rollout
> when config changes. Interviewers like this one.

---

## Section 3 — Release Lifecycle

```bash
helm install   cancellation ./cancellation-agent -f values-prod.yaml
helm upgrade   cancellation ./cancellation-agent -f values-prod.yaml --atomic --timeout 5m
helm rollback  cancellation 3                    # ← back to revision 3, in seconds
helm history   cancellation
helm uninstall cancellation

helm template  ./cancellation-agent -f values-prod.yaml   # render locally, apply nothing
helm lint      ./cancellation-agent
helm upgrade --install --dry-run --debug ...              # server-side dry run
```

### 3.1 A **release** is Helm's stateful concept

| Term | Meaning |
|---|---|
| **Chart** | the package (templates + defaults) |
| **Values** | the parameters for one install |
| **Release** | a named, versioned installation of a chart in a cluster |
| **Revision** | one version of a release — `helm rollback` targets these |

Helm stores release history as Secrets in the cluster. That is what makes `helm rollback` instant —
it re-applies a previously rendered manifest.

| Concept | Helm | Terraform | Bicep |
|---|---|---|---|
| Package | Chart | Module | Module |
| Parameters | `values.yaml` | `.tfvars` | `.bicepparam` |
| Deployed instance | **Release** | State/workspace | Deployment |
| Preview | `helm template` / `--dry-run` | `plan` | `what-if` |
| Undo | **`helm rollback`** ✅ | re-apply old code | redeploy |

**`--atomic` is the flag to remember:** if the upgrade fails or times out, Helm automatically rolls
back. Without it a failed upgrade leaves you half-deployed.

---

## Section 4 — GitOps and ArgoCD

### 4.1 Push vs pull — the model change

```
PUSH  (Azure DevOps — what you do today)
  pipeline holds cluster credentials  →  kubectl apply / helm upgrade  →  cluster
  ⚠️ CI has write access to prod. Drift is invisible. Nobody notices a manual kubectl edit.

PULL  (GitOps — ArgoCD / Flux)
  pipeline  →  builds image, updates a tag in a Git repo  →  STOPS
                                                              │
  ArgoCD (inside the cluster) watches Git  ◄──────────────────┘
      every ~3 min: compare Git vs cluster
      differs? → reconcile back to Git
  ✅ CI never touches the cluster. Git is the single source of truth.
     Manual changes are auto-reverted.
```

### 4.2 An ArgoCD Application

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: cancellation-agent
  namespace: argocd
spec:
  project: ai-platform
  source:
    repoURL: https://github.com/jma/ai-platform-manifests
    targetRevision: main
    path: charts/cancellation-agent
    helm:
      valueFiles: [values-prod.yaml]
  destination:
    server: https://kubernetes.default.svc
    namespace: ai-prod
  syncPolicy:
    automated:
      prune: true          # delete resources removed from Git
      selfHeal: true       # revert manual kubectl changes
    syncOptions: [CreateNamespace=true]
```

### 4.3 Why this matters for AI workloads specifically

`VitalCare:1488–1500` already documents the payoff:

- **Prompt versioning** — prompts live in Git, ArgoCD deploys them. A bad prompt is a `git revert`.
- **Model pinning** — the LiteLLM routing config is Git-managed; rolling back a model version is a
  commit, not a portal click.
- **Rollback in minutes** — *"ArgoCD rollback to previous Helm release for the orchestration
  service; covers tool definitions, routing logic, guardrail configuration."*

An auditor asking *"who changed the guardrail config and when?"* gets `git log`. That is the
compliance argument for GitOps, and it is stronger than the convenience argument.

### 4.4 App-of-apps

One root Application pointing at a folder of Applications. Bootstrapping a whole environment becomes
a single `kubectl apply`, and the environment definition is itself in Git.

### 4.5 ArgoCD vs Flux

| | ArgoCD | Flux |
|---|---|---|
| UI | strong web UI with a topology graph | CLI-first |
| Model | Application CRD | Kustomization / HelmRelease CRDs |
| Multi-tenancy | AppProjects, mature RBAC | namespace-scoped |
| Adoption | more common in enterprises | lighter, more composable |

The JD names ArgoCD. Learn ArgoCD; know Flux exists.

---

## Section 5 — AKS vs EKS vs GKE

Kubernetes itself is identical. What differs is everything around it — and that is where interview
questions live.

| | **AKS** (yours) | **EKS** (AWS) | **GKE** (GCP) |
|---|---|---|---|
| Control plane cost | free | **~$0.10/hr per cluster** | free (Autopilot charges per pod) |
| Identity to cloud | **Workload Identity** (Entra) | **IRSA** — IAM Roles for Service Accounts | Workload Identity |
| Secrets | Key Vault CSI driver | Secrets Manager / SSM CSI | Secret Manager CSI |
| Node autoscale | Cluster Autoscaler / NAP | Cluster Autoscaler / **Karpenter** | Autopilot / autoscaler |
| Registry | ACR | ECR | Artifact Registry |
| Ingress | App Gateway Ingress / NGINX | ALB Controller | GCLB |
| Auth | **Entra ID + PIM** *(you use this)* | IAM + `aws-auth` / access entries | Google IAM |
| Reputation | good Azure integration | most configuration required | most "batteries-included" |

### 5.1 The three you should be able to speak to

**IRSA** — AWS's answer to Workload Identity. A Kubernetes ServiceAccount is annotated with an IAM
role ARN; pods using it receive scoped AWS credentials automatically. No secrets in the cluster.

```yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: agent
  annotations:
    eks.amazonaws.com/role-arn: arn:aws:iam::123456789012:role/agent-bedrock-read
```

**Karpenter** — EKS node provisioning that picks instance types per pending pod rather than scaling
fixed node groups. For AI workloads it is the difference between "scale a GPU node group" and "get
exactly the GPU this pod asked for, spot if possible." Strong talking point for a GPU-serving
question.

**GKE Autopilot** — no node management at all; you pay per pod. Simplest for a small team.

**Interview line:**
> "Kubernetes is Kubernetes. What differs is identity, autoscaling and registry integration. I use
> AKS with Entra Workload Identity and the Key Vault CSI driver; the EKS equivalent is IRSA plus the
> Secrets Manager CSI driver, and for node scaling I'd look at Karpenter rather than fixed node
> groups, especially for GPU inference where instance type matters per workload."

---

## Section 6 — Service Mesh

### 6.1 What it is

A sidecar proxy (Envoy) next to every pod, intercepting all traffic. That gives you, **without
changing application code**:

| Capability | Why it matters for agents |
|---|---|
| **mTLS everywhere** | encrypted pod-to-pod; a compliance checkbox for HIPAA/PHI |
| **Retries, timeouts, circuit breaking** | infra-level version of `L31` §2's Polly patterns |
| **Traffic splitting** | 5% canary to a new agent version — real A/B on live traffic |
| **Automatic telemetry** | golden signals per service with no instrumentation |
| **Authorization policy** | "only the orchestrator may call the pricing agent" |

### 6.2 The options

| | Istio | Linkerd | Cilium |
|---|---|---|---|
| Power | highest | moderate | eBPF, no sidecar |
| Complexity | **high** | **low** | medium |
| Sidecar | Envoy (or ambient mode) | lightweight Rust | none |

### 6.3 When NOT to use one — say this part

> "A mesh solves cross-cutting network concerns for a *lot* of services. Under about ten services,
> the operational cost outweighs the benefit — you get the same retries and timeouts from Polly or a
> resilience library, and mTLS from an ingress plus network policy. I'd reach for a mesh when I need
> mTLS everywhere for compliance, traffic splitting for canaries, or authorization between many
> internal services. For a handful of agent services, no."

Knowing when a tool is overkill reads as more senior than knowing the tool.

---

## Section 7 — OpenShift (Awareness)

Red Hat's Kubernetes distribution. Runs on-prem **and** in cloud — that is the "hybrid" in the
glossary.

| | Vanilla K8s | OpenShift |
|---|---|---|
| Install | you assemble it | opinionated, batteries included |
| CI/CD | bring your own | built-in (Tekton, S2I) |
| Registry | external | built-in |
| Security | permissive by default | **SCC — restricted by default**; containers cannot run as root |
| Routing | Ingress | **Route** (own CRD) |
| Managed forms | AKS/EKS/GKE | **ARO** (Azure), **ROSA** (AWS) |

**The practical gotcha:** OpenShift's Security Context Constraints block containers running as root
and assign a random UID. Many public images assume root and simply fail. Charts often need
`runAsNonRoot` and writable-path fixes.

**Why enterprises use it:** a single Kubernetes API across on-prem datacenter and multiple clouds,
with Red Hat support. Common in regulated industries — exactly the "infrastructure organization"
the JD describes.

---

## Section 8 — GitHub Actions (tracker row 26)

Your Azure DevOps knowledge transfers almost directly.

| Azure DevOps | GitHub Actions |
|---|---|
| `trigger:` | `on:` |
| `pool: vmImage:` | `runs-on:` |
| `stages:` → `jobs:` → `steps:` | `jobs:` → `steps:` |
| `task: X@1` | `uses: org/action@v4` |
| `script:` | `run:` |
| Variable groups / Library | Secrets and Variables |
| Environments + approvals | Environments + protection rules |
| Service connection | OIDC federated credential |

```yaml
name: deploy-agent
on:
  push: { branches: [main] }

permissions:
  id-token: write            # ← required for OIDC. No stored cloud secrets.
  contents: read

jobs:
  deploy:
    runs-on: ubuntu-latest
    environment: prod        # gates + approvals
    steps:
      - uses: actions/checkout@v4
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - run: az aks get-credentials -g rg-ai -n aks-ai-prod
      - run: helm upgrade --install cancellation ./charts/cancellation-agent
                 -f values-prod.yaml --atomic --timeout 5m
```

**OIDC federated credentials** are the modern pattern — the pipeline exchanges a short-lived GitHub
token for a cloud token. No long-lived secret is stored anywhere. Worth naming in an interview.

---

## JM Family Anchor

You have the cluster. This is the exercise that turns this module from reading into evidence.

| Step | Command |
|---|---|
| 1. See what is deployed | `kubectl get deploy,svc,cm -n <ns>` |
| 2. Find what is Helm-managed | `helm list -A` — anything absent is raw YAML |
| 3. Inspect a release's values | `helm get values <release> -n <ns>` |
| 4. See the rendered manifest | `helm get manifest <release> -n <ns>` |
| 5. Diagnose a pod | `kubectl describe pod <p>` → Events at the bottom |
| 6. Chart your own service | scaffold with `helm create`, replace templates |
| 7. Render without deploying | `helm template ./chart -f values-dev.yaml` |

Step 5 is what you already did on the the `CrashLoopBackOff` you diagnosed — and the RBAC
Role/RoleBinding that was blocked on a blocked change request is precisely the kind of object that belongs in
a chart under GitOps: a reviewed pull request instead of a ticket waiting on another team.

---

## Self-Test Questions

1. What problem does Helm solve that raw Kubernetes YAML cannot?
2. Chart vs Release vs Revision — define each.
3. What does `--atomic` do on `helm upgrade`, and why should it be a default?
4. Why does changing a ConfigMap not restart pods, and what is the standard fix?
5. Push vs pull deployment — name the security advantage of pull.
6. What do `prune: true` and `selfHeal: true` do in an ArgoCD Application?
7. Someone edits a Deployment with `kubectl` on a GitOps-managed cluster. What happens?
8. AKS Workload Identity — what is the EKS equivalent, and how is it wired?
9. What does Karpenter do differently from the Cluster Autoscaler, and why does that matter for GPU
   inference?
10. Name three things a service mesh gives you without code changes — and one case where you would
    not use one.
11. Why do many public container images fail on OpenShift?
12. What is an OIDC federated credential in a pipeline, and what problem does it remove?

---

## Quick-Reference Interview Answers

**"Explain GitOps."**
> "Git is the source of truth and an agent inside the cluster continuously reconciles reality to
> match it. The shift from my Azure DevOps pipelines is push to pull: CI no longer holds cluster
> credentials — it updates an image tag in a manifests repo and stops. ArgoCD notices and syncs. Two
> things that buys me: manual `kubectl` changes get reverted automatically with `selfHeal`, so drift
> can't accumulate silently, and rollback is a `git revert`. For AI workloads that's especially
> valuable because prompts, tool definitions and model routing config are all just files — so a bad
> prompt is a revert, and an auditor asking who changed a guardrail gets `git log`."

**"How do you deploy an agent to Kubernetes?"**
> "Container image in ACR, a Helm chart with per-environment values files, and ArgoCD syncing from
> Git. Chart covers Deployment, Service, HPA and the Key Vault CSI SecretProviderClass so no secrets
> sit in the cluster. Readiness and liveness probes on a health endpoint, resource requests and
> limits set — requests especially, because without them the scheduler can't place pods sensibly.
> Config changes hash into a pod annotation so a ConfigMap edit actually triggers a rollout.
> `helm upgrade --atomic` so a failed deploy rolls itself back."

**"When would you use a service mesh?"**
> "When I need mTLS everywhere for compliance, traffic splitting for canary releases, or
> authorization policy between many internal services. Under about ten services I wouldn't — the
> operational cost outweighs it, and I'd get retries and timeouts from a resilience library and mTLS
> from ingress plus network policy instead."

---

## Related

`L33` (provisions the cluster this module deploys into) · `L31` §2–5 (retry, circuit breaker,
three-layer observability) · `L36` (tracing the workloads deployed here) · `L19` (CI/CD) ·
`VitalCare_AI_Assessment_Response.md:1488–1500, :1540–1560` (ArgoCD rollback, cloud-agnostic stack)
