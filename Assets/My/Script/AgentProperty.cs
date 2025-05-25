using UnityEngine;

namespace C2025_00 
{
    public enum PathfindingStrategy
    {
        ShortestPath,
        LeastCrowded,
        FollowGroup,
        RandomWalk
    }

    public enum AgentRole
    {
        Individual,
        GroupLeader,
        GroupFollower,
        Security,
        Rescue
    }


    [System.Serializable]
    public class AgentProperty
    {
        // 1. Physical Attributes
        public float maxSpeed;           // 걷기 최대 속도 (m/s)
        public float minSpeed;           // 걷기 최소 속도 (m/s)
        public float acceleration;       // 가속도
        public float deceleration;       // 감속도
        public float height;             // 키
        public float bodyRadius;         // 충돌 범위
        public float fieldOfViewAngle;   // 시야 각도

        // 2. Psychological Attributes
        public float patience;           // 인내심 (0 ~ 1)
        public float aggressiveness;     // 공격성 (0 ~ 1)
        public float goalFocus;          // 목표 집중도 (0 ~ 1)
        public float herdingTendency;    // 무리 행동 성향 (0 ~ 1)

        // 3. Behavioral Preferences
        public PathfindingStrategy pathStrategy;  // 경로 선택 전략
        public float obstacleAvoidanceSensitivity; // 장애물 회피 민감도
        public float personalSpace;               // 사회적 거리 (m)

        // 4. Social Attributes
        public string groupId;           // 그룹 ID
        public AgentRole role;           // 에이전트 역할
        public float socialInteractionSensitivity; // 다른 사람과의 반응 민감도

        // 5. Emotional State (시간에 따라 변할 수 있음)
        public float stressLevel;        // 스트레스 (0 ~ 1)
        public float fearLevel;          // 공포감 (0 ~ 1)
        public float calmness;           // 침착함 (0 ~ 1)

        // 6. Environmental Responsiveness
        public float perceptionSensitivity;  // 위험 인식 민감도
        public float adaptability;           // 경로 변경 시의 적응력

        public int stream_id;
        public Vector3 inflow_pos;
        public Vector3 outflow_pos;
        public Vector3 outflow_radius;

        
        // 생성자 (기본값 지정 가능)
        public AgentProperty()
        {
            maxSpeed = 1.5f;
            minSpeed = 0.5f;
            acceleration = 1.0f;
            deceleration = 1.0f;
            height = 1.7f;
            bodyRadius = 0.3f;
            fieldOfViewAngle = 120f;

            patience = 0.5f;
            aggressiveness = 0.3f;
            goalFocus = 0.7f;
            herdingTendency = 0.6f;

            pathStrategy = PathfindingStrategy.ShortestPath;
            obstacleAvoidanceSensitivity = 0.8f;
            personalSpace = 0.5f;

            groupId = "";
            role = AgentRole.Individual;
            socialInteractionSensitivity = 0.5f;

            stressLevel = 0.0f;
            fearLevel = 0.0f;
            calmness = 1.0f;

            perceptionSensitivity = 0.8f;
            adaptability = 0.7f;
        }
    }

    // 추가 열거형 정의

}