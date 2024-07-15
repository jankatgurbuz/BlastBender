// using System;
// using Cysharp.Threading.Tasks;
// using Global.Controller;
// using MenuScene.MenuPanel.Controller;
//
// namespace MenuScene.NavigatorPanel.View
// {
//     public interface INavigationPanelController
//     {
//         public void SmoothChange(int index);
//         public void DirectChange(int index);
//     }
//     public class NavigationPanelController : INavigationPanelController 
//     {
//         private readonly NavigationView _navigationView;
//         private readonly IMenuPanelController _menuPanelController;
//
//         public NavigationPanelController(NavigationView navigationView, IMenuPanelController menuPanelController)
//         {
//             _navigationView = navigationView;
//             _menuPanelController = menuPanelController;
//         }
//
//         public void SmoothChange(int index)
//         {
//             _navigationView.ChangeNavigation(index);
//             _menuPanelController.SmoothSlide(index);
//         }
//
//         public void DirectChange(int index)
//         {
//             _navigationView.ChangeNavigation(index);
//             _menuPanelController.DirectSlide(index);
//         }
//
//
//     }
// }
