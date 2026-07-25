# Module 5 — Speech Services
**Part 1: AI Fundamentals | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Matters

Speech services convert between human voice and text — the input/output layer for voice-driven AI. At JM Family, the most relevant use cases are:

- **Call center analytics** — transcribe dealer support calls, analyze sentiment
- **Meeting transcription** — transcribe internal meetings, extract action items
- **Accessibility** — convert AI text responses to speech for voice interfaces
- **Multilingual support** — translate dealer calls in real time

Speech feeds into NLP (Module 4) and ultimately into LLMs — voice → text → LLM → text → voice is the full pipeline for voice AI.

---

**Running example:**
> *JM Family dealer support receives 500+ phone calls per day. They want to transcribe calls, detect sentiment, extract key issues, and surface them to managers — without listening to every call.*

---

## Topic 5.1 — Speech Concepts

---

### 1. The Speech Pipeline

```
SPEECH-TO-TEXT (STT) PIPELINE:
  Microphone / Audio file
      │
      ▼ Audio processing
  PCM audio signal (16kHz, 16-bit, mono — standard format)
      │
      ▼ Acoustic model
  Phonemes (sound units) identified
      │
      ▼ Language model
  Words and phrases assembled from phonemes
      │
      ▼ Text output
  "The Ford F-150 delivery was three weeks late."

TEXT-TO-SPEECH (TTS) PIPELINE:
  Text string
      │
      ▼ Text normalization
  "March 15" → "March fifteenth"  |  "F-150" → "F one fifty"
      │
      ▼ Phoneme generation
  Text → phoneme sequence
      │
      ▼ Neural voice synthesis
  Phonemes → natural-sounding audio waveform
      │
      ▼ Audio output
  .wav / .mp3 / streaming audio
```

---

### 2. Key Speech Tasks

| Task | Direction | JM Family Use |
|---|---|---|
| **Speech-to-Text (STT)** | Audio → Text | Transcribe dealer support calls |
| **Text-to-Speech (TTS)** | Text → Audio | Read AI responses aloud in voice apps |
| **Speech Translation** | Audio → Text (different language) | Translate dealer call from Spanish to English in real time |
| **Speaker Recognition** | Audio → Speaker identity | Verify dealer rep identity on phone |

---

### 3. Audio Format Requirements

Azure AI Speech works best with:

```
Format:    WAV (PCM)
Sample rate: 16,000 Hz (16kHz) — minimum; 44.1kHz or 48kHz also works
Bit depth:  16-bit
Channels:   Mono (single channel) — for recognition
            Stereo only if you need speaker separation

For phone calls (8kHz):
  Phone audio is typically 8kHz — still works but lower accuracy
  Use telephony acoustic model (SpeechConfig.SetSpeechSynthesisVoiceName → telephony scenario)
```

---

## Topic 5.2 — Azure AI Speech Service

---

### 1. Service Capabilities Overview

```
AZURE AI SPEECH SERVICE
│
├── Speech-to-Text
│   ├── Real-time recognition (microphone / streaming)
│   ├── Batch transcription (audio files at scale)
│   ├── Custom Speech (domain vocabulary, acoustic adaptation)
│   └── Fast transcription (faster than real-time for files)
│
├── Text-to-Speech
│   ├── Neural voices (natural-sounding, 400+ voices, 140+ languages)
│   ├── SSML (Speech Synthesis Markup Language — fine control)
│   ├── Custom Neural Voice (your brand's voice)
│   └── Personal Voice (clone a voice from a sample)
│
├── Speech Translation
│   ├── Real-time (streaming audio in → text out in target language)
│   └── Batch (audio files translated asynchronously)
│
└── Speaker Recognition
    ├── Verification (is this person who they claim to be? 1:1)
    └── Identification (which of these enrolled speakers is this? 1:many)
```

---

### 2. Speech-to-Text — Real-Time Recognition

```csharp
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

// Setup
var config = SpeechConfig.FromEndpoint(
    new Uri("wss://eastus.stt.speech.microsoft.com/speech/universal/v2"),
    "<key>"
);
// Or with Managed Identity (use token-based auth)
config.SpeechRecognitionLanguage = "en-US";
config.SetProperty(PropertyId.SpeechServiceResponse_PostProcessingOption, "TrueText");

// From microphone
using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
using var recognizer = new SpeechRecognizer(config, audioConfig);

// One-shot recognition
var result = await recognizer.RecognizeOnceAsync();

if (result.Reason == ResultReason.RecognizedSpeech)
    Console.WriteLine($"Recognized: {result.Text}");
else if (result.Reason == ResultReason.NoMatch)
    Console.WriteLine("No speech recognized.");
else
    Console.WriteLine($"Error: {CancellationDetails.FromResult(result).ErrorDetails}");
```

---

### 3. Speech-to-Text — From Audio File (Call Recording)

```csharp
// From audio file (JMA call recording)
var audioConfig = AudioConfig.FromWavFileInput("/recordings/dealer-call-20260630.wav");
using var recognizer = new SpeechRecognizer(config, audioConfig);

// Continuous recognition (full recording, not just one phrase)
var allText = new StringBuilder();

recognizer.Recognized += (s, e) =>
{
    if (e.Result.Reason == ResultReason.RecognizedSpeech)
        allText.AppendLine(e.Result.Text);
};

recognizer.SessionStopped += (s, e) =>
{
    Console.WriteLine("Recognition complete.");
};

await recognizer.StartContinuousRecognitionAsync();
// ... wait for session to stop ...
await recognizer.StopContinuousRecognitionAsync();

Console.WriteLine($"Full transcript:\n{allText}");
```

---

### 4. Batch Transcription — Scale for Call Centers

For 500+ calls/day, real-time recognition per file is too slow. Use Batch Transcription:

```
Batch Transcription pattern:
  1. Upload audio files to Azure Blob Storage
  2. Submit batch transcription job (REST API call)
  3. Job runs asynchronously — can process 1000s of files in parallel
  4. Poll for completion (or use Event Grid notification)
  5. Download transcription results (JSON with text + timestamps + confidence)

JMA call center pipeline:
  Phone system → call recordings → Blob Storage
      → Batch Transcription job (nightly)
      → Transcripts (JSON) → Azure AI Language (sentiment, key phrases)
      → Results → Cosmos DB / Power BI dashboard
```

```csharp
// Submit batch transcription via REST
var batchRequest = new
{
    contentUrls = new[]
    {
        "https://jmastorage.blob.core.windows.net/callrecordings/call-001.wav",
        "https://jmastorage.blob.core.windows.net/callrecordings/call-002.wav"
    },
    locale = "en-US",
    displayName = "JMA Dealer Calls 2026-06-30",
    properties = new
    {
        wordLevelTimestampsEnabled = true,
        displayFormWordLevelTimestampsEnabled = true,
        diarizationEnabled = true,   // speaker separation
        maxSpeakerCount = 2          // assume 2 speakers (agent + dealer)
    }
};

var response = await httpClient.PostAsJsonAsync(
    "https://eastus.api.cognitive.microsoft.com/speechtotext/v3.1/transcriptions",
    batchRequest
);
```

---

### 5. Diarization — Speaker Separation

Diarization separates who said what in a multi-speaker recording:

```
Without diarization:
  "Hello this is dealer ATL-001 calling about my order yes I understand the delay..."

With diarization:
  Speaker 1 (00:00): "Hello, this is dealer ATL-001 calling about my order."
  Speaker 2 (00:05): "Yes, I understand the delay."
  Speaker 1 (00:08): "Can you tell me when it will arrive?"
  Speaker 2 (00:11): "The expected delivery is now July 15th."
```

**JMA call center use:** Separate agent vs dealer lines. Analyze dealer sentiment specifically (not agent). Identify if agent followed script.

---

### 6. Text-to-Speech — Neural Voices

```csharp
var config = SpeechConfig.FromSubscription("<key>", "eastus");
config.SpeechSynthesisVoiceName = "en-US-JennyNeural"; // choose a neural voice

using var synthesizer = new SpeechSynthesizer(config);

// Simple synthesis
var result = await synthesizer.SpeakTextAsync(
    "Your order for the F-150 has been confirmed. Delivery is expected on July 15th."
);

if (result.Reason == ResultReason.SynthesizingAudioCompleted)
    Console.WriteLine("Synthesis complete — audio played.");
```

**Available voice styles** (for Jenny Neural and others):
- `cheerful` — upbeat customer service
- `sad` — empathetic
- `angry` — not recommended for customer service
- `newscast` — formal, news-style
- `customerservice` — friendly, helpful

---

### 7. SSML — Fine Control Over Speech

SSML (Speech Synthesis Markup Language) lets you control exactly how text is spoken:

```xml
<speak version="1.0" xmlns="http://www.w3.org/2001/10/synthesis"
       xmlns:mstts="http://www.w3.org/2001/mstts" xml:lang="en-US">
  <voice name="en-US-JennyNeural">
    <mstts:express-as style="customerservice">
      Thank you for calling JM Family Dealer Support.
      <break time="500ms"/>
      Your order number is
      <say-as interpret-as="characters">ATL-001-2024</say-as>.
      <break time="300ms"/>
      The F-150 is expected to arrive on
      <say-as interpret-as="date" format="mdy">07/15/2026</say-as>.
      <prosody rate="slow" pitch="+2%">
        Is there anything else I can help you with today?
      </prosody>
    </mstts:express-as>
  </voice>
</speak>
```

SSML lets you control: voice, style, breaks, pronunciation, rate, pitch, volume, emphasis. Essential for production TTS.

---

### 8. Custom Speech — Domain Vocabulary

Standard STT struggles with:
- Brand names: "iPacket", "AutoNation", "JM Family"
- Automotive jargon: "floorplan", "MSRP", "PDI" (Pre-Delivery Inspection)
- Names: "Balaji", "Waterman", "Clement"

**Custom Speech fixes this by training on your audio data:**

```
Training data needed:
  Acoustic data: 1+ hour of relevant audio + transcripts
    (call recordings with accurate transcripts)
  Language data: plain text sentences with domain vocabulary
    (dealer communications, automotive terminology lists)

Workflow:
  Speech Studio (speech.microsoft.com)
  → Custom Speech
  → Create project
  → Upload training data
  → Train custom model
  → Evaluate: Word Error Rate (WER) — baseline vs custom
  → Deploy to custom endpoint

In your app:
  config.EndpointId = "<custom-endpoint-id>";
  // Now your STT knows "iPacket" and "JM Family"
```

**Word Error Rate (WER):** % of words incorrectly transcribed. Lower = better.
- Baseline (standard model): WER ~15% on automotive calls
- Custom model: WER ~7% (typical improvement with good training data)

---

## Topic 5.3 — Speech Translation & Speaker Recognition

---

### 1. Real-Time Speech Translation

Translates spoken audio directly to text in another language — no intermediate step needed:

```
Dealer speaks Spanish → Azure Speech Translation → English text (and/or audio)

One API call:
  Audio in (Spanish) → Translated text out (English)
  Optionally: also synthesize the translated text to audio
```

```csharp
var speechConfig = SpeechConfig.FromSubscription("<key>", "eastus");
speechConfig.SpeechRecognitionLanguage = "es-ES"; // input: Spanish
speechConfig.AddTargetLanguage("en");              // output: English

using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
using var recognizer = new TranslationRecognizer(speechConfig, audioConfig);

recognizer.Recognized += (s, e) =>
{
    if (e.Result.Reason == ResultReason.TranslatedSpeech)
    {
        Console.WriteLine($"Spanish: {e.Result.Text}");
        Console.WriteLine($"English: {e.Result.Translations["en"]}");
    }
};

await recognizer.StartContinuousRecognitionAsync();
```

---

### 2. Multi-Language Translation in One Call

```csharp
// Translate to multiple languages simultaneously
speechConfig.AddTargetLanguage("en"); // English
speechConfig.AddTargetLanguage("fr"); // French
speechConfig.AddTargetLanguage("de"); // German

// One audio input → three translation outputs simultaneously
```

---

### 3. Speaker Recognition

#### Speaker Verification (1:1)
"Is this person who they claim to be?"

```
Enrollment phase:
  Dealer rep enrolls: records 3 passphrases (10+ seconds of audio)
  Voice profile created: acoustic fingerprint of their voice

Verification phase:
  Dealer calls in: "My name is John Smith, dealer ATL-001"
  Compares voice to enrolled John Smith profile
  Returns: Accept (confidence 0.92) or Reject
```

```csharp
var client = new SpeakerRecognitionClient(config, audioConfig);

// Verify against enrolled profile
var result = await client.VerifySpeakerAsync(
    audio,
    profileId: "john-smith-voice-profile-id"
);

Console.WriteLine($"Result: {result.Reason}");      // Accepted / Rejected
Console.WriteLine($"Score: {result.Score:P0}");     // Confidence
```

#### Speaker Identification (1:many)
"Which of our enrolled dealers is this?"

```csharp
// Check against all enrolled profiles
var result = await client.IdentifySpeakerAsync(
    audio,
    profileIds: new[] { profile1, profile2, profile3 }
);

Console.WriteLine($"Identified: {result.ProfileId}");
Console.WriteLine($"Score: {result.Score:P0}");
```

---

### 4. Voice Profiles and Enrollment

```
Steps:
1. Create a voice profile (returns a profile ID)
2. Enroll: user records audio samples (minimum 15-20 seconds for verification)
3. Profile is ready for verification/identification

Profiles persist in the Speech resource — store the profile ID in your user database.

JMA use: Dealer authentication on phone IVR system.
  Instead of PIN: "Say your passphrase to confirm your identity"
  More secure than PIN (can't be shoulder-surfed), faster than passwords
```

---

### 5. Full JMA Call Center Pipeline

```
JMA Dealer Call Center — AI-Powered Pipeline
─────────────────────────────────────────────────────────────────

Phone call arrives
    │
    ▼
Azure Communication Services (phone integration)
    │ streams audio
    ▼
Azure AI Speech — Real-time Transcription
    │ with diarization (agent vs dealer speaker separation)
    ▼
Transcript chunks (streaming, ~2-5 second segments)
    │
    ├──► Azure AI Language — Sentiment Analysis (real-time)
    │         → Alert if dealer sentiment goes very negative
    │         → Trigger supervisor alert
    │
    └──► Buffer full transcript
              │
    ──────────────────────── (after call ends) ──────────────────
    │
    ▼
Full transcript → Azure AI Language
    ├── Key Phrase Extraction → what was the call about?
    ├── NER → VehicleModel, DealerCode, OrderNumber mentioned
    ├── Sentiment → overall call sentiment
    └── Summarization → 2-sentence call summary
    │
    ▼
Structured call record → Cosmos DB
    ├── Transcript (full text)
    ├── Summary
    ├── Entities found
    ├── Sentiment score
    └── Duration, date, dealer ID
    │
    ▼
Power BI Dashboard
    → Daily sentiment trends by dealer
    → Top issues (entity frequency)
    → Agent performance (resolution rate)
    → Call volume by category
```

---

## Topic R5 — Recall: Module 5 Review & Quiz

---

**Q1.** JM Family wants to transcribe 1,000 call recordings overnight. Should you use real-time recognition or batch transcription? Why?

> **A:** Batch transcription. Real-time recognition processes one file at a time, sequentially — 1,000 calls could take hours. Batch transcription submits all files at once and Azure processes them in parallel. You submit the job, go home, and results are ready in the morning. Batch also gives you richer output options (word-level timestamps, diarization, confidence per word).

---

**Q2.** Your STT is transcribing "iPacket" as "I packet" and "JM Family" as "J.M. Family" — inconsistent. What's the solution?

> **A:** Custom Speech. Upload training data from JMA call recordings with accurate transcripts. Add a language model text file with domain vocabulary: "iPacket", "JM Family", "AutoNation", "MSRP", "PDI", etc. Train a custom model in Speech Studio. Deploy to a custom endpoint. Use that endpoint ID in your SpeechConfig. WER should drop significantly for JMA-specific terminology.

---

**Q3.** What is diarization and why does it matter for JMA call analysis?

> **A:** Diarization separates who said what in a multi-speaker recording — labeling each utterance as "Speaker 1" or "Speaker 2". For JMA, this lets you analyze dealer sentiment separately from agent sentiment. You can tell if the dealer was upset vs the agent, whether the agent followed the escalation script, and whether issues were resolved based on dealer sentiment trajectory through the call.

---

**Q4.** What is SSML and when do you need it instead of plain TTS?

> **A:** SSML (Speech Synthesis Markup Language) is an XML format that gives fine control over how text is spoken — breaks, pronunciation, speaking rate, pitch, style, emphasis. You need it when: (1) text contains abbreviations that should be spelled out ("ATL-001" → "A-T-L-zero-zero-one"), (2) dates/numbers need specific reading ("07/15/2026" → "July fifteenth, twenty-twenty-six"), (3) you need pauses between sections, (4) you want the voice to express a specific style (customer service, empathetic). Plain `SpeakTextAsync` is fine for simple, clean text.

---

**Q5.** What is the difference between Speaker Verification and Speaker Identification?

> **A:** Verification (1:1) answers "Is this the person they claim to be?" — you compare the incoming voice against one specific enrolled profile. Used for identity confirmation. Identification (1:many) answers "Which of these people is this?" — you compare against multiple profiles and return the best match. Used when you don't know who is calling but want to match them to your enrolled speakers. Both require a voice enrollment phase where the person records audio samples in advance.

---

## Memory Hooks

- **"STT = Audio → Text, TTS = Text → Audio"**
- **"Real-time = one file now, Batch = 1000 files tonight"**
- **"Diarization = who said what"**
- **"Custom Speech = lower WER for your domain vocabulary"**
- **"WER = Word Error Rate — lower is better"**
- **"SSML = XML wrapper for fine-grained TTS control"**
- **"Verification = 1:1 (is this John?), Identification = 1:many (who is this?)"**
- **"Speech → Language → LLM: voice is the input layer, NLP is the processing layer"**

---

## 2026 Updates

| Topic | Update |
|---|---|
| **Custom Neural Voice** | Now requires Limited Access application (like Face ID) — Microsoft protecting against voice cloning misuse |
| **Personal Voice** | New feature — clone a voice from a short audio sample (also Limited Access) |
| **Fast Transcription** | New API — transcribes audio faster than real-time (for batch jobs where speed matters) |
| **Real-time diarization** | Now GA (was preview) — speaker separation works in real-time streaming, not just batch |
| **Azure Communication Services** | Now tightly integrated with Speech — phone calls can stream directly to Speech API without recording files first |

---

## Interactive Learning Ideas

### Exercise 1 — Speech Studio Exploration (15 min)
Go to speech.microsoft.com → Speech Studio:
- Try the Speech-to-Text demo with your own voice
- Try Text-to-Speech with different voices and styles
- Try the Real-time Speech Translation demo (speak English, get Spanish output)
- Compare: Jenny Neural vs Guy Neural vs Aria Neural voices

### Exercise 2 — Transcribe a JMA Call Scenario (20 min)
Record a 30-second mock dealer support call (you play both roles). Upload the WAV file to Speech Studio:
- Run standard STT — note any errors
- Note if automotive terms are wrong
- Enable diarization — does it correctly separate speakers?

### Exercise 3 — SSML Practice (15 min)
Write SSML for the following TTS output:
> "Your order number A-T-L-0-0-1-dash-2-0-2-6 has been confirmed. Delivery is expected on July fifteenth. (pause) Thank you for choosing JM Family."

Convert it to SSML with: `say-as` for order number, `say-as` for date, `break` for pause, customer service style.

### Exercise 4 — JMA Call Center Architecture (15 min)
Design on paper (or a whiteboard): the full pipeline from phone call arriving at JMA to a Power BI dashboard showing daily dealer sentiment trends. Include every Azure service, the data format at each step, and where data is stored. Then compare to the pipeline diagram in Topic 5.3.

### Exercise 5 — Custom Speech Planning
For JMA's call center, what training data would you need to build a Custom Speech model?
- What audio files would you use? (How many hours?)
- What text data would you include?
- How would you measure improvement? (What's your baseline WER?)
- Where would you get accurate transcripts for training?

---

*Previous: Module 4 — Natural Language Processing*
*Next: Module 6 — Azure Machine Learning*
*Connects to: Module 4 (NLP — speech feeds into language analysis), Module 7 (Custom models), Module 14 (AI Agents — voice as input channel)*
