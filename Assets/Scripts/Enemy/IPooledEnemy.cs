using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemy
{
	public interface IPooledEnemy
	{
		// Used for doing any initialization required after being grabbed from pool
		public void OnPoolGet();
		// Used for any clean-up 
		public void ReleaseToPool();
	}
}
