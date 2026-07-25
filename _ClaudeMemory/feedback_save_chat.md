---
name: feedback-save-chat
description: "When user says 'save the chat', append new messages to the PrepPlan file — never create a new file"
metadata: 
  node_type: memory
  type: feedback
  originSessionId: 4017a56a-8fd3-4aa8-ad15-83d63e8c9730
  modified: 2026-07-18T23:23:35.859Z
---

Where "save the chat" goes depends on the session topic.

**Health sessions** (prediabetes, acidity/reflux, bloating, diet) — **APPEND** to the existing thread, never create a new file:
`/mnt/c/pers/Health_Prediabetes_Acidity_ChatHistory_2026-07-09.md`
Append under a `# Session — YYYY-MM-DD (topic)` heading. Companion files live in `/mnt/c/pers/`.
Note: the `/share` skill's default script writes a NEW file into the AIML-Learn folder — do NOT run it as-is for health sessions; adapt it to append to the path above.

**AIML learning sessions**, save to:

**Folder:** `/mnt/c/Users/confksq/Project/AIML-Learn/07_ChatHistory/`  
**New file per session** — name it descriptively based on session topic + date.

⚠️ The `/share` skill's default script hardcodes the save dir to `PartsModules/` — that folder no longer exists after the 2026-07-18 reorganization, and the bug is what left a loose `Session_ChatHistory_2026-07-10.md` sitting among the lesson files. Always override the script's `save_dir` to `07_ChatHistory/` before running it.

Example: `ChatHist_AILearning_ProgressReview_AI102_2026-06-30.md`

**For old Ascendion prep sessions** (prior to 2026-06-30), the old file was:
`/mnt/c/pers/Job/AscendionIntr/PrepPlan/AscendionPrep_Day3_Module04-05_FoundryAgentSetup_RAGvsFinetune_2026-06-20.md`

**Why:** User moved from Ascendion interview prep to AIML learning path — chat history should now live with the learning project files.
