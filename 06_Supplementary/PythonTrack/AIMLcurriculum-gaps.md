# AI/ML Curriculum – Missing Topics (Gap Fill)

> Topics not covered in the main curriculum. Add these to become role-agnostic.

---

## Gap 1: SQL & Data Querying
- 1.1 SQL Basics – SELECT, WHERE, ORDER BY, LIMIT
- 1.2 Joins – INNER, LEFT, RIGHT, FULL OUTER
- 1.3 Aggregations – GROUP BY, HAVING, COUNT, SUM, AVG
- 1.4 Window Functions – ROW_NUMBER, RANK, LAG, LEAD, PARTITION BY
- 1.5 Subqueries & CTEs (Common Table Expressions)
- 1.6 Query optimization – indexes, execution plans

---

## Gap 2: HuggingFace Ecosystem
- 2.1 `transformers` library – `pipeline`, `AutoModel`, `AutoTokenizer`
- 2.2 `datasets` library – loading, streaming, mapping, filtering
- 2.3 `tokenizers` – fast tokenizers, padding, truncation
- 2.4 `Trainer` API – training arguments, callbacks, evaluation
- 2.5 `accelerate` – multi-GPU, mixed precision, device placement
- 2.6 `evaluate` – metrics (BLEU, ROUGE, accuracy, F1)
- 2.7 `peft` – LoRA, prefix tuning via HuggingFace
- 2.8 Model Hub – pushing/pulling models, model cards

---

## Gap 3: Experiment Tracking & Model Registry
- 3.1 MLflow – tracking (runs, params, metrics, artifacts)
- 3.2 MLflow – model registry (staging, production, versioning)
- 3.3 MLflow – model serving (`mlflow models serve`)
- 3.4 Weights & Biases (W&B) – runs, sweeps, dashboards
- 3.5 DVC (Data Version Control) – data versioning, pipelines, remote storage

---

## Gap 4: Model Serving & Inference Optimization
- 4.1 FastAPI – building model serving REST APIs
- 4.2 Flask – lightweight model APIs
- 4.3 ONNX – model export, ONNX Runtime inference
- 4.4 Quantization – INT8, FP16, dynamic vs static quantization
- 4.5 Pruning – structured vs unstructured, magnitude pruning
- 4.6 Knowledge distillation – teacher-student training
- 4.7 TensorRT – GPU inference optimization (NVIDIA)
- 4.8 vLLM / TGI – high-throughput LLM inference servers
- 4.9 Batching strategies – dynamic batching, continuous batching

---

## Gap 5: NLP Fundamentals (Pre-Transformer)
- 5.1 Text preprocessing – tokenization, stopwords, stemming, lemmatization
- 5.2 Bag of Words, TF-IDF
- 5.3 Word embeddings – Word2Vec (CBOW, Skip-gram), GloVe, FastText
- 5.4 Classical NLP tasks – POS tagging, NER, parsing (spaCy)
- 5.5 Sequence-to-sequence basics

---

## Gap 6: Generative AI Beyond LLMs
- 6.1 Generative Adversarial Networks (GANs) – generator, discriminator, training instability
- 6.2 Variational Autoencoders (VAEs) – encoder, latent space, decoder
- 6.3 Diffusion models – forward/reverse process, DDPM, DDIM
- 6.4 Stable Diffusion – UNet, VAE, CLIP text encoder, scheduler
- 6.5 Multimodal models – CLIP (image-text alignment), LLaVA, GPT-4V
- 6.6 Image generation APIs – DALL-E 3, Azure OpenAI image generation

---

## Gap 7: Async Python & API Patterns
- 7.1 `asyncio` – event loop, coroutines, `async`/`await`
- 7.2 `aiohttp` – async HTTP client/server
- 7.3 `httpx` – async-capable HTTP client
- 7.4 Background tasks – `asyncio.create_task`, task queues (Celery, RQ)
- 7.5 Streaming responses – SSE (Server-Sent Events), WebSockets

---

## Gap 8: Reinforcement Learning Basics
- 8.1 Core concepts – agent, environment, state, action, reward, policy
- 8.2 Markov Decision Process (MDP)
- 8.3 Q-Learning, Deep Q-Network (DQN)
- 8.4 Policy gradient methods – REINFORCE, Actor-Critic (A2C, A3C)
- 8.5 Proximal Policy Optimization (PPO) – in depth (underpins RLHF)
- 8.6 Gymnasium (OpenAI Gym) – environment setup and training

---

## Gap 9: Non-Azure Cloud (AWS & GCP)
- 9.1 AWS SageMaker – training jobs, endpoints, pipelines, model registry
- 9.2 AWS Bedrock – managed LLM APIs (Claude, Llama, Titan)
- 9.3 AWS S3, Lambda, ECR basics
- 9.4 GCP Vertex AI – training, prediction, pipelines, model garden
- 9.5 GCP BigQuery ML – in-database ML models
- 9.6 GCP Cloud Storage, Cloud Run basics

---

## Gap 10: DSA for Coding Interviews
- 10.1 Arrays & strings – two pointers, sliding window
- 10.2 Hash maps & sets
- 10.3 Linked lists, stacks, queues
- 10.4 Trees & graphs – BFS, DFS, recursion
- 10.5 Sorting & searching – binary search, merge sort
- 10.6 Dynamic programming – memoization, tabulation
- 10.7 Complexity analysis – time/space trade-offs

---

*Pair this file with `AIMLcurriculum-outline.md` for the full picture.*
