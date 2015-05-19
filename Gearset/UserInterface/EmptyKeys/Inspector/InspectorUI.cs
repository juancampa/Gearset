//#if EMPTYKEYS
//    using System;
//    using EmptyKeys.GearsetUI;
//    using EmptyKeys.GearsetUI.Model;
//    using EmptyKeys.UserInterface.Generated;

//    namespace Gearset.UserInterface.EmptyKeys
//    { 
//        public class InspectorUI 
//        {
//            readonly InspectorWindow _inspectorWindow;
//            readonly InspectorViewModel _inspectorViewModel;

//            public InspectorUI(InspectorWindow inspectorWindow, InspectorViewModel inspectorViewModel)
//            {
//                _inspectorWindow = inspectorWindow;
//                _inspectorViewModel = inspectorViewModel;
//            }

//            public void Inspect(string name, Object o, bool autoExpand, Type t)
//            {
//                var treeNode = new InspectorNode { Name = name};
//                _inspectorViewModel.TreeItems.Add(treeNode);
//                _inspectorWindow.TreeView.ItemsSource = null;
//                _inspectorWindow.TreeView.ItemsSource = _inspectorViewModel.TreeItems;
//            }
//        }
//    }
//#endif
