# BEAT IT

## Your music is the level.

A synthwave motorcycle runner where the road, the collectibles, and the traffic are all generated from a song you actually own. Drop in any MP3, and the game listens to it — beat by beat — to build a highway that only exists because that specific track does.



## What it actually does
Import any Mp3 file. It Analyses it and uses the data to generate a level that you can speed through in style and rhythm.

No two playthroughs look the same, because no two songs are the same.

## How to play

Controls: A/D or Left/Right to weave across the road.

Your music: On first launch, hit Play, then "Open Songs Folder" on the song select screen. Drop your MP3s in, back out to the main menu, hit Play again to refresh the list.



## Tech
Unity, C# (new Input System)
URP, with a Volume-driven vignette for the hit-feedback effect
No third-party DSP libraries — the beat and melody detection are hand-rolled: single-pole low-pass/band-pass filtering, spectral-flux-style onset detection, and a lightweight spectral-centroid approximation for melody direction, all built directly on raw AudioClip sample data
