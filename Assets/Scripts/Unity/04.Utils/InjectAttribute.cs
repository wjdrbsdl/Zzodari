using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[AttributeUsage(AttributeTargets.Field)] //이 어트리뷰트는 필드 에만 붙일 수 있다.
public class InjectAttribute : Attribute
{
}
