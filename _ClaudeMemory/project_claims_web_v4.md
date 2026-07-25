---
name: project-claims-web-v4
description: jma-claims-automation repo cloned; web-v4 created from v3 with working API integration; app registrations already exist in Entra ID
metadata: 
  node_type: memory
  type: project
  originSessionId: ddc5c8bc-e0c0-4b13-b236-daab25e09fbe
---

Repo `JMA-Apps/jma-claims-automation` cloned to `C:\Users\confksq\source\repos\JMA-Apps\jma-claims-automation` (2026-07-13). Created `jma-mechanical-claims-web-v4` (copy of v3, Next.js — NOT Angular; the Angular 6 app is `jma-mechanical-claims-web` v1) per Gary's request, with: implicit-flow login (redirect + `#access_token` capture → sessionStorage) in `src/lib/auth/loginFlow.ts`, dev proxy rewrites in `next.config.ts` (`/api/claimsprocess/*` → `${API_PROXY_TARGET}/api/v1/*`, config → `public/dev-config.json`).

**No new app registration needed** — existing pair in tenant e2ba673a-b782-4f44-b0b5-93da90258200: SPA "JMA-Mechanical Claim-Dev" (b907bb86-8ccb-49cc-b93b-092ce7580f50, implicit access-token enabled) and API "JMA-Mechanical Claim API-Dev" (ba31bba3-..., `api://jmfamily.com/jma/mechanicalclaimapi-dev`, scope `Admins`, app roles `rw`/`ro`). Only `http://localhost:3000/adminconsole/` needs adding as SPA redirect URI (not yet done — user to confirm).

Vitest on WSL /mnt/c is extremely slow (jsdom setup >5s/file) — v4 vitest.config.ts sets testTimeout/hookTimeout 60s; test "failures" at default timeout are environment flakes, not code bugs.
