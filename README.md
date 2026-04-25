# 💬 ChatTalk

> TCP 기반 실시간 채팅 프로그램

---

## 📌 프로젝트 소개

ChatTalk은 C#(.NET) 기반으로 구현한 TCP 소켓 통신 채팅 프로그램입니다.

단순 메시지 송수신을 넘어 다음 기능을 구현했습니다.

* 멀티 클라이언트 처리
* 명령어 기반 프로토콜 설계
* 귓속말 기능
* 사용자 상태 관리

---

## 🛠 기술 스택

* Language: C#
* Framework: .NET
* UI: WPF
* Network: TCP Socket

---

## 🏗 아키텍처

```
ChatTalk
├── ChatTalk.Client   (WPF UI)
├── ChatTalk.Server   (TCP Server)
└── ChatTalk.Common   (Protocol / Parser / Builder)
```

---

## 📦 패키지 구조

### 🔹 Client

* UI → 화면 (MainWindow, ChatWindow)
* Network → 서버 통신 (Client)
* Command → 사용자 입력 처리
* Model → UI 데이터 모델

### 🔹 Server

* Network → TCPServer / ClientHandler

### 🔹 Common

* Building → MessageBuilder
* Parsing → MessageParser
* Messages → ProtocolMessage 계열

---

## ⚙️ 주요 기능

### 1. 멀티 클라이언트 처리

* ConcurrentDictionary 기반 사용자 관리
* 비동기 Task 기반 메시지 처리

---

### 2. 프로토콜 기반 통신

```
MSG^||^sender^||^messageId^||^content
JOIN^||^user
LEAVE^||^user
USRLIST^||^user1,user2
WHISPER^||^from^||^to^||^message
```

---

### 3. 귓속말 기능

```
/w 대상유저 메시지
```

---

### 4. 사용자 상태 관리

* 입장 / 퇴장 메시지 처리
* 사용자 목록 실시간 갱신

---

## 🧠 설계 포인트

* Builder / Parser 패턴 적용
* DTO 기반 프로토콜 구조 설계
* UI / 네트워크 책임 분리

---

## 🚀 실행 방법

1. 서버 실행 (ChatTalk.Server)
2. 클라이언트 실행 (ChatTalk.Client)
3. 접속 정보 입력

```
IP   : localhost
PORT : 서버 실행 포트
```
