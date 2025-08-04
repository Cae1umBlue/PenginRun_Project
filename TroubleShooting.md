# 트러블 슈팅(Trouble Shooting) 모음

## 목차
1. 아이템 스폰 리팩토링
2. 슬라이드 상태에서 점프가 비정상적으로 동작하며, 콜라이더 오프셋이 꼬이는 문제
3. 블럭 벽을 올라타는 문제
4. 사운드 오브젝트 생성과 파괴, 가비지 컬렉션
5. 5.씬 재시작 후 SoundManager와 슬라이더 연결 끊김 문제

---

## 1. 아이템 스폰 리팩토링

### 문제상황
<img width="1173" height="515" alt="image" src="https://github.com/user-attachments/assets/6fae965b-62b9-40e8-8d03-69b45b043c00" />
<img width="1089" height="507" alt="image" src="https://github.com/user-attachments/assets/fc2cb393-ecb4-4d5f-9341-ace89b0888a6" />

- 인스펙터 작업 번거로움
  - 스폰 포인트 하나 추가할 때마다 spawnPoints 배열 크기를 늘리고, 각 요소에 Transform과 ItemType을 일일이 드래그 & 드롭해야 함.
- 타입 분기 중복
  - switch–case 블록이 매번 반복되며, 새 아이템 타입이 추가될 때마다 두 곳(스크립트, 프리팹 필드)에 수정을 요구함
- 유지보수성 저하
  - 씬 뷰에서 위치만 바꿔도 배열 설정이 깨질 수 있고, 멤버 순서가 바뀌면 엉뚱한 타입이 배치될 위험.

---

 ### 개선점
 
<img width="831" height="488" alt="image" src="https://github.com/user-attachments/assets/90fd6ac5-24c5-4303-93f1-4dc637d355d8" />
<img width="926" height="641" alt="image" src="https://github.com/user-attachments/assets/16de7a8d-72f3-4a1c-80f6-f0fb4516f35c" />

- 타입별로 부모(컨테이너)만 지정 → 각 컨테이너의 자식 위치를 한 번에 순회해 Spawn
- 배열 대신 Transform 필드 4개로 단순화

---

### 개선효과
<img width="599" height="423" alt="image" src="https://github.com/user-attachments/assets/6a93372e-89df-415f-b428-30644e922371" />

- 인스펙터 단순화
  - 포인트 추가/삭제 시 배열 크기 조절 불필요.
  - 씬 뷰에서 컨테이너(ScorePointsParent 등) 아래에 빈 오브젝트만 추가하면 끝
- 코드 중복 제거
  - switch 분기를 없애고, SpawnFromParent() 한 번만 작성.
- 확장성 향상
  - 새 아이템 타입 추가 시 컨테이너·프리팹 필드만 추가하고, Start()에 한 줄만 더 등록

---

## 2. 슬라이드 상태에서 점프가 비정상적으로 동작하며, 콜라이더 오프셋이 꼬이는 문제

### 문제상황

<img width="600" height="390" alt="image" src="https://github.com/user-attachments/assets/f4fac821-3b51-42ea-9aad-c412a5d0572d" />

- 프레임 타이밍 문제, 입력 우선순위의 모호함
  - IsSliding = false직후 Jump()가 호출되면서 콜라이더가 이미 점프용 오프셋으로 바뀌어 슬라이드 콜라이더 복원보다 먼저 실행
- 콜라이더 복원 로직의 중복 실행
  - RestoreColliderOffsetIfNeeded()는 Update()마다 실행되지만, 슬라이드와 점프가 동시에 일어나면, 콜라이더가 중간 상태에 빠지며 충돌 판정이 꼬인다
 
### 개선점 및 효과

<img width="607" height="398" alt="image" src="https://github.com/user-attachments/assets/97edbaa1-db9b-47db-b959-f6014869bfd9" />

- 슬라이드 상태 전환 → 점프 시도 → 콜라이더 상태 정합성 유지 흐름을 보장하는 방식으로 코드를 조정
  - Jump() 자체에서 슬라이드 중이면 아예 실행되지 않도록 강제함으로써, 모든 점프 경로를 통합 관리.
  - RestoreColliderOffsetIfNeeded() 호출은 슬라이드 상태와 무관하게 오프셋을 복원하려 하기 때문에, 이 또한 점프와 겹치면 오류가 발생할 수 있다. 따라서 삭제 처리함
 
---

## 3. 블럭 벽을 올라타는 문제

### 문제상황 

<img width="530" height="262" alt="image" src="https://github.com/user-attachments/assets/d5e1c77a-ee7d-4d0c-a046-6a88b0d13e49" />

- 블럭벽과 충돌이 날 경우 점프 카운트가 초기화
- 지면이 아닌 벽과의 충돌도 점프 카운트가 초기화 됨
- 충돌 구역이 1개여서 코드를 건들어야 하는지 고민

### 개선버전

<img width="600" height="309" alt="image" src="https://github.com/user-attachments/assets/dfa7c93e-a617-4a59-9143-f6e11f6a6d68" />

- 충돌 구역을 2개로 분리
  - Collision은 점프 카운트를 초기화 하지 않음
  - Block만 점프 카운트를 초기화
- 모서리 부분까지 반토막 블럭으로 처리
  - 모서리 벽타기도 불가능하게 구현

  ---

## 4. 사운드 오브젝트 생성과 파괴, 가비지 컬렉션

### 문제상황

<img width="1746" height="744" alt="carbon" src="https://github.com/user-attachments/assets/f7bffda4-10fb-4ae9-a842-3d83a2966758" />

- 메서드 호출 시 Sound Object를 새로 생성 - 역할 수행 - 오브젝트 파괴 형식으로 구현
- 수많은 오브젝트 생성과 파괴로 인한 성능저하 및 Garbage Collection(이하 GC) 발생 
- 로직과 코드 수정의 필요성을 느낌

### 개선점

<img width="1662" height="664" alt="carbon (1)" src="https://github.com/user-attachments/assets/45fa7b2b-2a79-4be5-8bc1-60df68b16a00" />


- SoundManager의 자식 Object로 한번만 생성
- 채널의 갯수만큼 AudioSource 부착 및 설정
- AudioClip을 생성한 AudioSource에 할당받아 재사용

### 개선효과

- 성능 저하 방지(CPU 부하)
- GC 발생 최소화
- Audio 재생의 안전성 증가

---

## 5.씬 재시작 후 SoundManager와 슬라이더 연결 끊김 문제

### 문제상황

<img width="1005" height="313" alt="image" src="https://github.com/user-attachments/assets/1b18ce15-c2c3-44d7-a926-5e46e1500e5f" />

- GameOver -> Restart -> Scene 초기화 시 참조 끊김
- SoundManager가 이전 슬라이더 인스턴스를 참조하고 있어 Missing 발생
- 슬라이더와 SoundManger간의 연결 유지 방법 필요

### 개선점


<img width="1290" height="984" alt="carbon (2)" src="https://github.com/user-attachments/assets/20fe7ce2-19c2-4dc5-9e83-dc11e8397fbe" />


- UIManager에서 직접 Slider 인스턴스 참조 후 전달
- Slider 초기화 시 이벤트 핸들러(BGM/SFXVolume)를 코드에서 직접 등록
- 전달받은 Slider를 내부에 저장하고 초기값 설정

### 개선효과

- 씬 재시작 후에도 Slider와 Audio 설정 정상 작동
- UI 참조 안정성 확보 및 이벤트 처리 일관성 향상
- 사용자 설정 자동 반영으로 UX 개선

