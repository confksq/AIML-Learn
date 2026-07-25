# Layer 5: API Keys & Security

> Companion to: [05 — Layer 5: Deployment & Monitoring](05-Layer5-Deployment-and-Monitoring.md)

---

## API Key = Password for Your Endpoint

```
Regular App Login:          AI Endpoint Call:
──────────────────          ─────────────────
Username + Password   ───►  API Key
Grants access to app  ───►  Grants access to endpoint
```

### But More Specifically...

```
API Key is like a:

Password          ✅  proves you are allowed in
Service Account   ✅  not tied to a person, tied to an app
Master Key        ✅  one key = full access (no username needed)
```

### Where It Goes in the Request

```http
POST https://aiml-learn-resource.openai.azure.com/...
api-key: 3a8f2c••••••••e91b       ← just this, no username needed
```

> No login screen. No username. Just the key — and you're in.

---

## Question 1: Is It Hardcoded in Code?

```
❌ Should NEVER be hardcoded
✅ Should always come from Key Vault or Environment Variables

Bad (hardcoded):
─────────────────────────────────────────────
var apiKey = "3a8f2c••••••••e91b";   ← dangerous!

Good options:
─────────────────────────────────────────────
Option A — Environment Variable:
  var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

Option B — Azure Key Vault (best practice):
  var apiKey = await secretClient.GetSecretAsync("azure-openai-key");

Option C — Managed Identity (no key at all!):
  var credential = new DefaultAzureCredential();  ← zero secrets in code
  var client = new AzureOpenAIClient(endpoint, credential);
```

> **Managed Identity** is the gold standard — your app proves its identity via Azure AD,
> no key needed at all. Like Windows Authentication vs SQL login in .NET.

---

## Question 2: Can You Rotate It?

```
YES — and it's easy in AI Foundry Portal:

My assets → Models + endpoints → Keys

┌─────────────────────────────────┐
│  Key 1:  3a8f2c••••••e91b  🔄  │  ← Regenerate
│  Key 2:  9b7e4d••••••f82c  🔄  │  ← Regenerate
└─────────────────────────────────┘
```

### Why TWO Keys? — Zero Downtime Rotation

```
Step 1:  App uses Key 1 (active)
         Key 2 = spare

Step 2:  Regenerate Key 1
         Switch app to Key 2 (no downtime)

Step 3:  Regenerate Key 2
         Switch app back to Key 1

Result:  Key rotated, app never went down ✅
```

> Same pattern as Azure Storage Account keys — you already know this!

---

## Question 3: Does It Have Expiry?

```
Azure OpenAI API Keys:
 └── NO automatic expiry ❌
      └── They live forever until YOU rotate or delete them
      └── This is why rotation policy matters

BUT you can enforce expiry via:
 ├── Key Vault → set secret expiration date
 │    └── Alert fires when secret nears expiry
 │    └── Your pipeline auto-rotates before expiry
 │
 └── Azure Policy
      └── Enforce "keys must be rotated every 90 days"
```

---

## API Key vs Managed Identity

| | API Key | Managed Identity |
|---|---|---|
| **What it is** | Static secret (like password) | Azure AD token (auto-managed) |
| **Expires** | No (unless you set policy) | Yes — auto-refreshes every hour |
| **Rotation** | Manual or automated | Automatic — Azure handles it |
| **Risk if leaked** | High — works until rotated | Low — token expires in 1 hour |
| **Code needed** | Store & pass the key | Just `new DefaultAzureCredential()` |
| **Best for** | Quick dev/testing | Production always |

---

## JMA Best Practice Setup

```
Development:
 └── API Key stored in User Secrets (local)
      └── dotnet user-secrets set "AzureOpenAI:Key" "3a8f2c..."

Staging & Production:
 └── Managed Identity (no key at all)
      └── App Service → Identity → System Assigned = ON
      └── AI Foundry → Access control → Grant role to App Service
      └── Code: new DefaultAzureCredential()  ← done!

Key Vault (backup / extra security):
 └── Store key in Key Vault
 └── Set expiry = 90 days
 └── Azure Policy alerts at 30 days before expiry
 └── DevOps pipeline auto-rotates
```

---

## .NET Code Comparison

```csharp
// ❌ Bad — hardcoded (like writing password in source code)
var apiKey = "3a8f2c••••••••e91b";

// ✅ Good — Environment Variable
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

// ✅ Better — Azure Key Vault
var secretClient = new SecretClient(
    new Uri("https://kv-jma-dev-ai.vault.azure.net/"),
    new DefaultAzureCredential()
);
var apiKey = (await secretClient.GetSecretAsync("azure-openai-key")).Value.Value;

// ✅ Best — Managed Identity (zero secrets)
var credential = new DefaultAzureCredential();
var client = new AzureOpenAIClient(
    new Uri("https://aiml-learn-resource.openai.azure.com/"),
    credential   // ← no key needed at all
);
```

---

## Security Rules — Never Break These

```
❌  Never hardcode API keys in source code
❌  Never commit keys to GitHub (even private repos)
❌  Never share keys in emails, Teams, or Slack
❌  Never use same key across dev/stg/prod
❌  Never skip rotation because "nothing happened yet"

✅  Always use Key Vault or Managed Identity
✅  Always use separate keys per environment
✅  Always rotate keys every 90 days (or less)
✅  Always use Managed Identity in production
✅  Always set Key Vault expiry alerts
```

---

## One-Line Summary

> API Keys **don't expire by default** — you must rotate them manually or enforce via
> Key Vault policy. In production, skip keys entirely and use **Managed Identity**
> — zero secrets, auto-expiring tokens, no rotation headache.

---

## Navigation

| | |
|---|---|
| **Previous** | [05b — Layer 5: Endpoints Real World](05b-Layer5-Endpoints-RealWorld.md) |
| **Next** | [06 — Course Progress Recap](06-Course-Progress-Recap.md) |
