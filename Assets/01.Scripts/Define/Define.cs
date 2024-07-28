public class Define
{
    public enum Scene
    {
        Login,
        Loading,
        Main,
    }

    public enum MapControll
    {
        SumMonster,
        SumPlayerStairUp,
        SumPlayerStairDown,
        SumItem,
    }

    public enum CreatureState
    {
        Idle,
        Moving,
        Skill,
        Dead,
        None,
    }

    public enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public enum SkillList
    {
        Attack,
        Arrow,
        None,
    }
}
