using UnityEngine;

public class SpongeObj : MonoBehaviour
{
    public int id = 5; //스펀지 오브젝트 구별위해서 
    public SpongeObj leftSide;
    public SpongeObj rightSide;

    public float m_pressPower = 2f;
    public Vector3 m_pressDirec = Vector3.left;
    public float springLength = 5f;
    public float springConstant = 2f;
    public float curSpringLength = 5f;
    public float curSpringPower = 0;
    public Vector3 m_tailVec;
    public Vector3 m_headVec;
    public GameObject m_tailObj;
    public GameObject m_headObj;

    private void Start()
    {
        //꼬리 위치에 따라서 머리 위치 조정 
        m_headObj.transform.position = m_tailObj.transform.position + (m_pressDirec * -1 * springLength);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && id == 0) //아이디 0 인애만 조정하기
        {
            Press(m_pressPower, m_pressDirec, this);
        }
        if (Input.GetKeyUp(KeyCode.Space) && id == 0)//아이디 0 인애만 조정하기
        {
            손놓기(this);
        }

        Emit();
        RenderSpring();
    }

    void RenderSpring()
    {
        m_headObj.transform.position = m_tailObj.transform.position + (m_pressDirec * -1 * curSpringLength);
    }

    bool isPressing = false;

    public void 전파(SpongeObj _from, float _pressure, Vector2 _pressDirec)
    {
        isPressing = true;
        Press(_pressure, _pressDirec, _from);
    }

    public void 해방(SpongeObj _from)
    {
        손놓기(_from);
    }

    void Press(float _pressure, Vector2 m_pressDirec, SpongeObj _from)
    {
        isPressing = true;
        //누를때 
        //현재 저항력보다 누르는 힘이 쌔면은 계속 누르고 압축될때까지 압축 
        float 압축 = springLength - curSpringLength;
        float 반발력 = 압축 / springLength * springConstant; //압축된 비율에 스프링 상수가 파워
        float 여분 = _pressure - 반발력;
        curSpringLength -= 여분 * Time.deltaTime;
        Debug.Log(여분 * Time.deltaTime + "만큼 압축시켰다.");
        //내 힘과 탄성력이 겹쳐지는 순간 쪼그라드는거 중단

        if (leftSide != null && leftSide != _from)
        {
            leftSide.전파(this, _pressure * 0.8f, m_pressDirec);
        }
        if (rightSide != null && rightSide != _from)
        {
            rightSide.전파(this, _pressure * 0.8f, m_pressDirec);
        }
    }

    void 손놓기(SpongeObj _from)
    {
        isPressing = false;
        if (leftSide != null && leftSide != _from)
        {
            leftSide.해방(this);
        }
        if (rightSide != null && rightSide != _from)
        {
            rightSide.해방(this);
        }
    }

    void Emit()
    {
        if (isPressing == true)
        {
            return;
        }

        //압축되어 있었으면 그냥 방출 그 힘을 리턴
        float 압축된비율 = (springLength - curSpringLength) / springLength;

        float 스프링반발력 = Mathf.Lerp(0, springConstant * springConstant * springConstant * springConstant, 압축된비율);
        //반발력 은 압축된 비율 1에 가까울수록 곱으로 쌔지고, 비율0 에 가까울수록 순둥해질것. 
        //반발력은 springConstant의 제곱 - 그 

        // float preLength = curSpringLength;
        curSpringLength += 스프링반발력 * Time.deltaTime;
        //  Debug.Log(preLength +" 에서 튕겨져셔 "+curSpringLength);
    }
}
