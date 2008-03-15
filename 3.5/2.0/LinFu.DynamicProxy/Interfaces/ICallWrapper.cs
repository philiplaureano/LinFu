using System;

namespace LinFu.DynamicProxy
{		
	public interface IInvokeWrapper
	{
		void BeforeInvoke(InvocationInfo info);
		object DoInvoke(InvocationInfo info);
		void AfterInvoke(InvocationInfo info, object returnValue);
	}
}
