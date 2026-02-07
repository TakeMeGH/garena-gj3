using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public abstract class SlotMiniGame
    {
        float _elapsed;

        public void Start()
        {
            _elapsed = 0f;
            OnStart();
        }

        public void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
            OnTick(deltaTime);
        }

        public float Complete()
        {
            float score = GetScore();
            return Mathf.Clamp(score, 0f, 100f);
        }

        protected float Elapsed => _elapsed;

        protected abstract void OnStart();
        protected virtual void OnTick(float deltaTime) { }
        protected abstract float GetScore();
        public abstract string Name { get; }

        public virtual bool UsesBar => false;
        public virtual bool UsesTarget => false;
        public virtual float BarValue01 => 0f;
        public virtual Vector2 TargetPosition01 => Vector2.zero;
    }

    public sealed class BarPeakMiniGame : SlotMiniGame
    {
        float _period;
        float _offset;

        protected override void OnStart()
        {
            _period = Random.Range(0.9f, 1.4f);
            _offset = Random.Range(0f, _period);
        }

        protected override float GetScore()
        {
            return BarValue01 * 100f;
        }

        public override bool UsesBar => true;
        public override float BarValue01
        {
            get
            {
                float phase = (Elapsed + _offset) * Mathf.PI * 2f / _period;
                return (Mathf.Sin(phase) + 1f) * 0.5f;
            }
        }

        public override string Name => "Bar Peak";
    }

    public sealed class BowAndArrowMiniGame : SlotMiniGame
    {
        Vector2 _position;
        Vector2 _velocity;
        float _radius;
        float _maxSpeed;
        float _minSpeed;
        float _turnCooldown;

        protected override void OnStart()
        {
            _radius = Random.Range(0.8f, 1.2f);
            _minSpeed = 0.8f;
            _maxSpeed = 2.2f;
            _position = Random.insideUnitCircle * _radius;
            _velocity = Random.insideUnitCircle.normalized * Random.Range(_minSpeed, _maxSpeed);
            _turnCooldown = Random.Range(0.15f, 0.45f);
        }

        protected override void OnTick(float deltaTime)
        {
            float delta = Mathf.Max(deltaTime, 0f);
            _turnCooldown -= delta;
            if (_turnCooldown <= 0f)
            {
                Vector2 steer = Random.insideUnitCircle.normalized;
                _velocity = Vector2.Lerp(_velocity, steer * Random.Range(_minSpeed, _maxSpeed), 0.65f);
                _turnCooldown = Random.Range(0.15f, 0.45f);
            }

            _position += _velocity * delta;
            if (_position.magnitude > _radius)
            {
                _position = _position.normalized * _radius;
                _velocity = Vector2.Reflect(_velocity, _position.normalized);
            }
        }

        protected override float GetScore()
        {
            float distance = _position.magnitude / _radius;
            float score = Mathf.Clamp01(1f - distance);
            return score * 100f;
        }

        public override bool UsesTarget => true;
        public override Vector2 TargetPosition01 => _radius > 0f ? _position / _radius : Vector2.zero;
        public override string Name => "Bow And Arrow";
    }
}
