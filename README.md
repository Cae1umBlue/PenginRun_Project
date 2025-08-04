<div align="center">
  <img width="446" height="347" alt="PenguinRun" src="https://github.com/user-attachments/assets/89d621e0-60ed-4fad-8774-43916ebfe530" />
</div>
<div align="center">
<img src="https://img.shields.io/badge/Unity-222324?style=flat&logo=unity&logoColor=white"/>
<img src="https://img.shields.io/badge/-C%23-663399?logo=Csharp&style=flat&logoColor=white"/>
</div>

---

# 펭귄런(PenguinRun)

## 📖 목차
[프로젝트 개요](#프로젝트-개요)

[기술 스택](#기술-스택)

[클래스 다이어그램](#클래스-다이어그램)

[프로젝트 구조](#프로젝트-구조)

[트러블 슈팅](#트러블-슈팅)

[에셋 출처](#에셋-출처)

[팀원](#팀원)

---
    
## 프로젝트 개요

#### 코인을 획득하며 결슴점을 향해 달려가세요!
<img width="537" height="299" alt="image" src="https://github.com/user-attachments/assets/535c896e-a757-4af9-b148-698c58062a1f" />

| 항목            | 내용                                   |
|-----------------|--------------------------------------|
| **게임명**       | Penguin Run                          |
| **장르**         | 런 게임(Run Game), 플랫포머(Platformer) |
| **개발 기간**    | 2025.07.29(화) ~ 2025.08.04(월)       |
| **타겟 플랫폼**  | PC, Phone(Android, iOS)              |

#### 조작법
1) 자동으로 펭귄이 앞으로 이동합니다.
2) PC에서는 **점프(Jump) - SpaceBar**, **슬라이딩(Slide) - L-Shift** 를(을) 사용하여 조작할 수 있습니다.
3) 모바일 환경에서는 화면에 표시된 **점프(Jump)**와 **슬라이딩(Slide) 버튼**을 통해 조작할 수 있습니다.

#### 게임 시스템
1. **게임 시작**
   - 시작 화면에서 `Start` 버튼을 클릭하면 게임이 시작됩니다.

2. **게임 목표**
   - 현재 1개의 맵이 구현되어 있으며, **결승점까지 도착하는 것**이 1차 목표입니다.

3. **점수 획득**
   - 맵에 배치된 **코인** **(Coin)** 을 획득하여 점수를 얻을 수 있습니다.
   - 점수를 모아 **기록에 도전**하세요.

4. **아이템 활용**
   - 맵 중간중간에 다음과 같은 아이템이 배치되어 있습니다:
     - **HP 회복 아이템**
     - **펭귄의 이동속도 증가/감소 아이템**
   - 상황에 맞게 전략적으로 사용해주세요.

5. **게임 오버 조건**
   - 다음 중 하나에 해당하면 `Game Over`입니다:
     - 맵 바깥으로 떨어짐
     - 체력 게이지가 전부 소진됨
     - 결승점에 도착

6. **점수 기록**
   - `Game Over` 시점의 점수가 기록됩니다.
   - 점수가 **최고 기록을 갱신**했다면, 해당 점수가 **최고 점수로 저장**됩니다.

---

## 기술 스택

| 항목            | 내용                                   |
|-----------------|--------------------------------------|
| **Language**    | C#                                   |
| **Engine**      | Unity 2022.3.2f1                     |
| **IDE**         | Visual Studio 2022                   |
| **Target Platform**  | PC, Phone(Android, iOS)              |
| **Verson Control**  | Git, GitHub              |
| **Library**  | TextMeshPro              |

---

## 클래스 다이어그램
<img width="663" height="453" alt="image" src="https://github.com/user-attachments/assets/07553241-af9f-4b1f-acba-0a577936ad88" />

---

## 프로젝트 구조

### 📁 Assets
- Animations
- Artwork
- Materials
- Prefabs
- Scenes
  - StartScene
- Scripts
  - CameraController.cs
  - GameManager.cs
  - Item.cs
  - ItemSpawner.cs
  - Obstacle.cs
  - PlayerController.cs
  - ScoreManager.cs
  - SlideButtonHandler.cs
  - SoundManager.cs
  - UIManager.cs
- Sounds
- TextMesh Pro

---

## 트러블 슈팅
자세한 내용은 [트러블슈팅 문서](TroubleShooting.md)를 참고하세요.

---

## 에셋 출처
- 배경화면 : https://assetstore.unity.com/packages/2d/environments/pixel-skies-demo-background-pack-226622
- 캐릭터 : https://assetstore.unity.com/packages/2d/characters/2d-character-sprite-animation-penguin-236747
- 사운드
  - https://uppbeat.io/sfx/cartoon-falling-over-1/5080/19650
  - https://opengameart.org/sites/default/files/Win%20sound.wav
  - https://freepd.com/music/Rush.mp3
  - https://kenney.nl/assets/ui-audio
 
---
## 팀원
<img width="1221" height="439" alt="image" src="https://github.com/user-attachments/assets/6a98f1e9-1414-4c67-8466-6669fe840a50" />

- 이서준 (팀장) : https://gist.github.com/LeeSeoJune-stack
- 구슬기 : https://github.com/sulg16
- 김정은 : https://github.com/hsju3359
- 우승일 : https://github.com/NekrosGame
- 오승엽 : https://github.com/Cae1umBlue

