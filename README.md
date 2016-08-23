# WebApiServerBase 샘플 프로젝트

##### 구성
* ASP.NET Web API 프레임워크
* SignalR WebSocket

##### 요약
* ASP.NET Web API 기반의 서버를 제작할 때 사용하는 베이스 프레임입니다.
* Web API 프로젝트는 ApiController 및 주로 통신 및 사전 인증에 관련된 부분만 담당하고, 로직은 ServerCore 부분에 최대한 배치합니다.
* 채팅 등의 간단한 리얼타임 처리를 위한 웹소켓 기능(SignalR)이 포함되어 있습니다.

##### 통신 관련 및 기타
* ASP.NET Web API 에서 기본적으로 지원하는 json 등을 통한 통신이 가능하며, POST 방식으로 간단하게 패킷 단위의 구성을 할 때는 바이너리로 패킷을 제작합니다.
* 바이너리를 사용할 경우 구글 프로토버프를 사용하는 것이 범용성이 있으리라 예상합니다. (예시 프로젝트에서는 간단한 바이너리 시리얼라이저를 제작해서 적용했습니다)
* 예시 프로젝트에서는 Api의 RESTful 영역을 담당하는 ApiController가 표준적인 http api 처리를 담당하고, 채팅 등의 간단한 리얼타임 기능을 대응하기 위해 SignalR을 적용했습니다. SignalR은 웹 페이지를 비롯 Unity3D(어셋 스토어의 BestHTTP 플러그인)등에서도 사용 가능한 웹소켓 기반 통신 프레임워크입니다.
* 클라우드 서비스에 배포할 경우, Azure의 경우에는 포탈에서 제공하는 AlwaysOn 설정이 필요하고, Amazon Web Services에서는 관련 설정을 IIS에 직접 하는 것으로 상시 가동형 웹서버를 구성해야 합니다.
 + idletimeout, alwaysrunning, periodicrestart 등의 설정이 필요합니다.