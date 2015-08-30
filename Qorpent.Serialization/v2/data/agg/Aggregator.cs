using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent;
using Qorpent.Experiments;

namespace qorpent.v2.data.agg {
    public class Aggregator {
        private int kid = 0;
        private int mid = 0;
        private IScope _scope;

        public Aggregator()
        {
            Node = new AggregateNode();
            Routers = new List<IRouter>();
            Collectors = new List<ICollector>();
            Finalizers = new List<IFinalizer>();
        }
        protected AggregateNode Node { get; private set; }

        private IEnumerable<IRouter> AllRoutes(IRouter root = null) {
            if (null == root) {
                foreach (var router in Routers) {
                    foreach (var route in AllRoutes(router)) {
                        yield return route;
                    }
                }
            }
            else {
                yield return root;
                if (root.HasChildren) {
                    foreach (var router in root.Children) {
                        foreach (var route in AllRoutes(router)) {
                            yield return route;
                        }
                    }
                }

            }
        }  



        public AggregateNode GetResult() {
            Finalize();
            Node.Collectors = Collectors.ToArray();
            Node.Routs = AllRoutes().ToArray();
            

            return Node;
        }

        private void Finalize() {
            foreach (var finalizer in Finalizers) {
                finalizer.Execute(Node);
            }
        }

        public void Add(object obj, IScope scope = null) {
            var current = Node;
            scope = _scope ?? (_scope = scope ?? new Scope());
            SetValues(obj, scope, current);
           
            foreach (var retriever in Routers) {
                    
                ProcessRetriever(obj, scope, retriever, current);
            }
            
        }

        private void ProcessRetriever(object obj, IScope scope, IRouter retriever, AggregateNode current) {
            if (!retriever.Filter(obj, current, scope)) return;
            var subnode = string.IsNullOrWhiteSpace(retriever.Key) ? current : current.GetNode(retriever.Key);
            subnode.IsBucket = true;
            subnode.RouteKey = new RouteKey(retriever.Key);
            AggregateNode keynode = subnode;
            SetValues(obj, scope, subnode);
            var key = retriever.GetKey(obj, scope);
            if(null!=key) {
                keynode = subnode.GetNode(key.Key);
                keynode.RouteKey = key;
                SetValues(obj, scope, keynode);
            }
            if (retriever.HasChildren) {
                foreach (var r in retriever.Children  ) {
                    ProcessRetriever(obj,scope,r,keynode);
                }
            }
        }

        private void SetValues(object obj, IScope scope, AggregateNode current) {
            foreach (var collector in Collectors) {
                if(!collector.Filter(obj,current,scope))continue;
                
                var currentValue = current.GetValue(collector.Key);
                var newValue = collector.GetValue(obj, currentValue, current, scope);
                if (currentValue != newValue && null!=newValue) {
                    current.SetValue(collector.Key, newValue);
                }
            }
        }

        public List<IRouter> Routers { get; set; } 
        public List<ICollector> Collectors { get; set; } 
        public List<IFinalizer> Finalizers { get; set; } 
        public void Register(IRouter router) {
            if (string.IsNullOrWhiteSpace(router.Key))
            {
                router.Key = "agg" + kid++;
            }
            Routers.Add(router);
        }

        public void Register(Func<object, object, AggregateNode, IScope, object> valFunc)
        {
            Collectors.Add(new Collector {ValueFunction = valFunc, Key = "val"+mid++});
        }


        public void Register(IFinalizer finalizer)
        {
           
            Finalizers.Add(finalizer);
        }

        public void Register(Action<AggregateNode> finalizer)
        {
            Finalizers.Add(new Finalizer {ExecuteFunction = finalizer});
        }

        public void Register(ICollector collector)
        {
            if (string.IsNullOrWhiteSpace(collector.Key)) {
                collector.Key = "val" + mid++;
            }
            Collectors.Add(collector);
        }

        public void Register(Func<object, IScope, RouteKey> retriever)
        {
            Routers.Add(new Router(retriever) {Key = "agg"+kid++});
        }


    }
}