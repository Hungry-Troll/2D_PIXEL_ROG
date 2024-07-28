using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


// ��ɰ� UI�κи�
public class CreatureController : MonoBehaviour
{

    [SerializeField]
    protected float _speed = 3.0f;

    protected MoveDir _dir = MoveDir.None;
    //protected Vector3Int _cellPos = new Vector3Int(0, -1, 0);

    public Vector3Int CellPos { get; set; } = new Vector3Int(0, -1, 0);

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    [SerializeField]
    protected CreatureState _state = CreatureState.Idle;

    protected MoveDir dir; // ���� üũ��
    protected MoveDir SkillDir; // ��ų ���� üũ��
    protected SkillList skillList = SkillList.None; // ��ų����

    [SerializeField]
    protected GameObject _target;//�÷��̾�

    //���¸� �̿��ؼ� �ִϸ��̼� ���
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }
    [SerializeField]
    protected MoveDir _lastDir = MoveDir.None;
    //������ �̿��ؼ� �ִϸ��̼� ���
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            _dir = value;

            if (value != MoveDir.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (SkillDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
            case MoveDir.None:
                cellPos += Vector3Int.zero;
                break;
        }

        return cellPos;
    }

    //�ִϸ��̼� ���
    protected virtual void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            _animator.Play("idle");
            #region ���߿� 4���� ���� ����ϸ� �׶� ���

            /*            switch (_lastDir)
                        {
                            case MoveDir.Up:
                                _animator.Play("idle");
                                break;
                            case MoveDir.Down:
                                _animator.Play("idle");
                                break;
                            case MoveDir.Left:
                                _animator.Play("idle");
                                break;
                            case MoveDir.Right:
                                _animator.Play("idle");
                                break;
                        }*/
            #endregion
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("Run");
                    break;
                case MoveDir.Down:
                    _animator.Play("Run");
                    break;
                case MoveDir.Left:
                    _spriteRenderer.flipX = true;
                    _animator.Play("Run");
                    break;
                case MoveDir.Right:
                    _spriteRenderer.flipX = false;
                    _animator.Play("Run");
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            switch (SkillDir)
            {
                case MoveDir.Up:
                    _animator.Play("Attack");
                    break;
                case MoveDir.Down:
                    _animator.Play("Attack");
                    break;
                case MoveDir.Left:
                    _spriteRenderer.flipX = true;
                    _animator.Play("Attack");
                    break;
                case MoveDir.Right:
                    _spriteRenderer.flipX = false;
                    _animator.Play("Attack");
                    break;
                case MoveDir.None:
                    break;
            }
        }
        else if (State == CreatureState.None)
        {
            _animator.Play("idle");
        }
    }

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 pos = MapManager.Instance.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void CreatureIdle(IIdle Idle)
    {
        Idle.IdlePlay();
    }

    protected virtual void CreatureMove()
    {

    }

    protected virtual void CreatureSkill()
    {

    }

    protected virtual void CreatureDead()
    {

    }

    protected virtual void CreatureNone()
    {

    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
            case CreatureState.None:
                UpdateNone();
                break;
        }
    }

    //�̵������ѻ����϶� ���� ��ǥ�� �̵��Ѵ�.(���)
    protected virtual void UpdateIdle()
    {

    }

    protected virtual void UpdateMoving()
    {   //���
        Vector3 destPos = MapManager.Instance.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        //��������
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();

            Dir = MoveDir.None;
            dir = MoveDir.None;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }

    }

    protected virtual void MoveToNextPos()
    {

        if (_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }


        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
            case MoveDir.None:
                destPos += Vector3Int.zero;
                break;
        }


        if (MapManager.Instance.CanGo(destPos))
        {
            if (ObjectManager.Instance.Find(destPos) == null)
            {
                Debug.Log("������Ʈ�� ��� ��������");
                CellPos = destPos;
                //State = CreatureState.Moving;
            }
        }


        //���������� �ִϸ��̼��� ���� ��Ʈ��
        /*        _state = CreatureState.Idle;
                if (_dir == MoveDir.None)
                    UpdateAnimation();

                Dir = MoveDir.None;
                dir = MoveDir.None;*/
    }




    protected virtual void UpdateSkill() { }

    protected virtual void UpdateDead() { }

    protected virtual void UpdateNone() { }

    protected virtual void MonsterTargetSearch() { }


    public virtual void OnDamaged(int damage) { }
}
