using UnityEngine;

namespace BoschingMachine.Elevators
{
    public class Elevator : MonoBehaviour
    {
        [SerializeField] float smoothTime;
        [SerializeField] float maxSpeed;
        [SerializeField] float threshold = 0.1f;
        [SerializeField] int startingFloor;
        [SerializeField] float waitDuration;

        new Rigidbody rigidbody;
        float velocity;
        bool waiting;
        float waitTimer;
        ElevatorGroup group;

        int direction;
        bool wasWaiting;

        bool[] requests;

        bool Idle => waiting && waitTimer > waitDuration;

        public int TargetFloor { get; private set; }
        public int CurrentFloor { get; private set; }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            group = GetComponentInParent<ElevatorGroup>();

            requests = new bool[group.Floors.Length];
        }

        private void Start()
        {
            transform.position = HeightToPoint(group.Floors[startingFloor]);
            TargetFloor = startingFloor;
        }

        private void FixedUpdate()
        {
            CurrentFloor = GetCurrentFloor();

            float floorPoint = group.Floors[TargetFloor];
            waiting = Mathf.Abs(floorPoint - rigidbody.position.y) < threshold * threshold;

            if (waiting)
            {
                if (!wasWaiting)
                {
                    requests[TargetFloor] = false;
                    group.Requests[TargetFloor] = false;
                }

                waitTimer += Time.deltaTime;
                if (Idle)
                {
                    if (TryGetNextFloor(out var newFloor))
                    {
                        TargetFloor = newFloor;
                        direction = (int)Mathf.Sign(newFloor - TargetFloor);
                    }
                }
                rigidbody.MovePosition(HeightToPoint(floorPoint));
            }
            else
            {
                if (TryGetNextFloor(out var newFloor))
                {
                    if ((group.Floors[newFloor] - rigidbody.position.y) * direction > 0)
                    {
                        TargetFloor = newFloor;
                    }
                }

                waitTimer = 0.0f;
                float h = Mathf.SmoothDamp(rigidbody.position.y, floorPoint, ref velocity, smoothTime, maxSpeed);
                rigidbody.MovePosition(HeightToPoint(h));
            }

            wasWaiting = waiting;
        }

        private bool TryGetNextFloor(out int f)
        {
            var floors = group.Floors.Length;
            var requests = new bool[floors];

            var count = 0;
            for (int i = 0; i < floors; i++)
            {
                if (requests[i] = requests[i] || group.Requests[i]) count++;
            }

            foreach (var elevator in group.Elevators)
            {
                if (elevator == this) continue;

                requests[elevator.TargetFloor] = false;
            }

            f = -1;
            if (count == 0) return false;

            var fForward = CurrentFloor;
            var fBack = CurrentFloor;

            for (int i = CurrentFloor; i < floors; i++)
            {
                if (requests[i])
                {
                    fForward = i;
                    break;
                }
            }

            for (int i = CurrentFloor; i >= 0; i--)
            {
                if (requests[i])
                {
                    fBack = i;
                    break;
                }
            }

            if (fBack == CurrentFloor && fForward == CurrentFloor) f = CurrentFloor;

            else if (fBack == CurrentFloor) f = fForward;
            else if (fForward == CurrentFloor) f = fBack;

            else if (direction > 0) f = fForward;
            else if (direction < 0) f = fBack;

            else f = fForward - CurrentFloor < CurrentFloor - fBack ? fForward : fBack;

            return true;
        }

        public void Request(int i)
        {
            requests[i] = true;
        }

        public Vector3 HeightToPoint(float h) => new Vector3(transform.position.x, h, transform.position.z);

        public int GetCurrentFloor ()
        {
            int b = 0;
            for (int i = 0; i < group.Floors.Length; i++)
            {
                float d1 = Mathf.Abs(group.Floors[i] - rigidbody.position.y);
                float d2 = Mathf.Abs(group.Floors[b] - rigidbody.position.y);

                if (d1 < d2) b = i;
            }
            return b;
        }

        public static string IndexToFloorName(int i) => i == 0 ? "G" : $"L{i}";
    }
}
