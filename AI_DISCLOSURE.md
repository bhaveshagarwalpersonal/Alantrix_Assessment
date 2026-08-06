# AI_DISCLOSURE.md

I used Claude (Anthropic) during this project, specifically as follows:

- **Core architecture:** I consulted Claude when defining the core architecture and layering for the
  project (Domain / Matching / Scoring / Persistence / Presentation, and how they should hand off to each
  other), and used its definitions and structure as the starting point. From there, my own work is built
  on top of that foundation — the implementation, extension, and day-to-day development is mine.

- **Bug fixing and integration:** All bug fixing and integration work — including diagnosing the save/load
  sprite issue in `CardView`, the rotation desync in `CardAnimator`, tracing the data flow across
  `GameSessionController`/`SaveService`/`BoardGenerator`, and wiring the fixes back into the project — was
  done by me, without AI assistance.
-**Docs Writing:** All Docs provided were generated using Ai Tools Such as Chatgpt and Perplexity but were reviewed and edited properly by me

I'm happy to walk through any specific piece of the code in more detail if useful.

