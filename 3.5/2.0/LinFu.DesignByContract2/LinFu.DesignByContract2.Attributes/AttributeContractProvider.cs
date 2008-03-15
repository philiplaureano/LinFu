using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Attributes
{
    public class AttributeContractProvider : IContractProvider 
    {
        private MethodFinder _finder;
        private IContractTypeProvider _contractTypeProvider = new DefaultContractTypeProvider();

        public IContractTypeProvider ContractTypeProvider
        {
            get { return _contractTypeProvider; }
            set { _contractTypeProvider = value; }
        }

        #region IContractProvider Members

        public ITypeContract GetTypeContract(Type targetType)
        {
            IContractSource contractType = _contractTypeProvider.ProvideContractForType(targetType);
            object[] attributes = contractType.GetCustomAttributes(typeof(IInvariant), true);
            TypeContract contract = new TypeContract();
            foreach(object attribute in attributes)
            {
                IInvariant invariant = attribute as IInvariant;
                if (invariant == null)
                    continue;
                
                contract.Invariants.Add(invariant);
            }
            return contract;
        }

        public IMethodContract GetMethodContract(Type targetType, InvocationInfo info)
        {
            #region Build the list of base classes and interfaces from which to inherit contracts
            List<Type> combinedList = GetCombinedBaseList(targetType);

            #endregion
            
            MethodContract result = new MethodContract();
            MethodInfo targetMethod = info.TargetMethod;

            // Reuse the finder from the previous method call
            if (_finder == null)
                _finder = new MethodFinder();
            
            foreach(Type current in combinedList)
            {
                IContractSource contractType = _contractTypeProvider.ProvideContractForType(current);
                
                if (contractType == null)
                    continue;
                
                MethodInfo target = _finder.FindMatchingMethod(contractType, targetMethod);
                if (target == null)
                    continue;
                
                AttributeExtractor extractor = new AttributeExtractor();

                // Preconditions on each level of inheritance have to be AND'd together
                CompositePrecondition currentPrecondition = new CompositePrecondition();
                extractor.ExtractAttributesFrom(target, currentPrecondition.Preconditions);

                // Extract the parameter preconditions from the current method
                // and combine them with the existing preconditions
                foreach(ParameterInfo parameter in target.GetParameters())
                {
                    List<IParameterPrecondition> parameterPreconditions = new List<IParameterPrecondition>();
                    extractor.ExtractAttributesFrom(parameter, parameterPreconditions);
                    
                    foreach(IParameterPrecondition parameterPrecondition in parameterPreconditions)
                    {
                        CheckParameterPrecondition checkPrecondition =
                            new CheckParameterPrecondition(parameter, parameterPrecondition);
                        currentPrecondition.Preconditions.Add(checkPrecondition);                        
                    }
                }
                
                if (currentPrecondition.Preconditions.Count == 1)
                    result.Preconditions.Add(currentPrecondition.Preconditions[0]);
                
                if (currentPrecondition.Preconditions.Count > 1)
                    result.Preconditions.Add(currentPrecondition);
                
                // Extract the postcondition attributes attached to the method
                extractor.ExtractAttributesFrom(target, result.Postconditions);
                
                // Extract any additional postcondition attributes attached
                // to the return type
                extractor.ExtractAttributesFrom(target.ReturnTypeCustomAttributes, result.Postconditions);
            }
            
            return result;
        }

        private List<Type> GetCombinedBaseList(Type targetType)
        {
            List<Type> interfaceList = new List<Type>();
            AddInterface(targetType, interfaceList);

            List<Type> parentList = new List<Type>();
            Type currentType = targetType;
            while(currentType != null && currentType != typeof(object))
            {
                parentList.Add(currentType);
                currentType = currentType.BaseType;
            }

            List<Type> combinedList = new List<Type>();
            if (interfaceList.Count > 0)
                combinedList.AddRange(interfaceList);

            if (parentList.Count > 0)
                combinedList.AddRange(parentList);
            return combinedList;
        }

        private static void AddInterface(Type type, List<Type> interfaceList)
        {
            foreach(Type current in type.GetInterfaces())
            {
                AddInterface(current, interfaceList);
            }
            
            if (!interfaceList.Contains(type))
                interfaceList.Add(type);
        }
        #endregion
        
    }
}
