using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureData : MonoBehaviour
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

    protected MoveDir dir; // 방향 체크용
    protected MoveDir SkillDir; // 스킬 방향 체크용
    protected SkillList skillList = SkillList.None; // 스킬종류

    [SerializeField]
    protected GameObject _target;//플레이어

    //상태를 이용해서 애니메이션 재생
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            //UpdateAnimation();
        }
    }
}
