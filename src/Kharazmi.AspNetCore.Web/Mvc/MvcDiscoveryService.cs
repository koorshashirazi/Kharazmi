using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Web.Mvc
{
    internal static class MvcConstant
    {
        /// <summary>
        /// 
        /// </summary>
        public const string UnderLineSeparator = "_";

        /// <summary>
        /// 
        /// </summary>
        public const string SlashSeparator = "/";
    }

    internal static class Tree
    {
        public static MvcTreeViewModel BuildMvcTreeView(this List<MvcTreeViewModel> nodes, MvcTreeViewModel rootNode)
        {
            nodes.CheckArgumentIsNull(nameof(nodes));
            var rootChilds = nodes.FirstOrDefault(x => x.ControllerType == rootNode.ControllerType);
            var tree = rootNode.BuildTree(nodes);
            tree.Items.AddRange(rootChilds?.Items);
            return tree;
        }

        private static MvcTreeViewModel BuildTree(this MvcTreeViewModel root, List<MvcTreeViewModel> nodes)
        {
            if (nodes.Count == 0)
                return root ?? new MvcTreeViewModel();

            var children = root.FetchChildren(nodes).ToList();

            root.Items.AddRange(children);
            root.RemoveChildren(nodes);

            for (var i = 0; i < children.Count; i++)
            {
                children[i] = children[i].BuildTree(nodes);
                if (nodes.Count == 0)
                    break;
            }

            return root;
        }

        private static IEnumerable<MvcTreeViewModel> FetchChildren(this MvcTreeViewModel rootNode,
            IEnumerable<MvcTreeViewModel> nodes)
        {
            return nodes.Where(n => n.ParentControllerType == rootNode.ControllerType);
        }

        private static void RemoveChildren(this MvcTreeViewModel rootNode, ICollection<MvcTreeViewModel> nodes)
        {
            foreach (var node in rootNode.Items)
                nodes.Remove(node);
        }
    }

    #region Attributes

    /// <summary></summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MvcTreeViewAttribute : Attribute
    {
        /// <summary> </summary>
        public string Id { get; set; }

        /// <summary> </summary>
        public string GroupId { get; set; }

        /// <summary> Type of parent controller </summary>
        public Type ParentControllerType { get; set; }

        /// <summary>
        /// Filter controllers and action by special attribute
        /// </summary>
        public Type[] FilterByAttributes { get; set; }

        /// <summary>
        /// Set or get policy of authroz Authorize
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary> Set or get Title for action or controller </summary>
        public string Title { get; set; }

        /// <summary> Set or get Description for action or controller </summary>
        public string Description { get; set; }

        /// <summary> Set or get CssIcon for action or controller </summary>
        public string CssIcon { get; set; }

        /// <summary> Set or get CssClass for action or controller </summary>
        public string CssClass { get; set; }

        /// <summary></summary>
        public string ImageUrl { get; set; }

        /// <summary> Set or get Order for action or controller </summary>
        public int Order { get; set; }

        /// <summary> Set or get Visible for action or controller </summary>
        public bool Visible { get; set; } = true;

        /// <summary> Set or get Enable for action or controller </summary>
        public bool Enable { get; set; } = true;

        /// <summary> </summary>
        public bool Expanded { get; set; } = true;

        /// <summary> </summary>
        public bool Checked { get; set; } = false;

        /// <summary> </summary>
        public bool Selected { get; set; } = false;

        /// <summary> </summary>
        public bool IsClickable { get; set; } = true;

        /// <summary> </summary>
        public bool IsAjax { get; set; } = true;
    }

    /// <summary> </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SideBarAttribute : MvcTreeViewAttribute
    {
    }

    #endregion

    #region Models

    /// <summary> </summary>
    public class MvcTreeViewModel
    {
        /// <summary> </summary>
        public string Id { get; set; }

        /// <summary> </summary>
        public string GroupId { get; set; }

        /// <summary> </summary>
        public string Title { get; set; }

        /// <summary> </summary>
        public string Description { get; set; }

        /// <summary>  It's set to `{AreaName}/{ControllerName}/{ActionName}` </summary>
        public string Url { get; set; }

        /// <summary> </summary>
        public bool IsVisible { get; set; }

        /// <summary> </summary>
        public int Order { get; set; }

        /// <summary></summary>
        public string CssClass { get; set; }

        /// <summary> </summary>
        public string CssIcon { get; set; }

        /// <summary></summary>
        public string ImageUrl { get; set; }

        /// <summary> </summary>
        public bool Enabled { get; set; } = true;

        /// <summary></summary>
        public bool Expanded { get; set; } = true;

        /// <summary></summary>
        public bool Selected { get; set; } = false;

        /// <summary></summary>
        public bool Checked { get; set; } = false;

        /// <summary> </summary>
        public bool IsAjax { get; set; } = true;

        /// <summary> </summary>
        public bool IsClickable { get; set; } = true;

        /// <summary>Return ControllerActionDescriptor.ActionName </summary>
        public string ActionName { get; set; }

        /// <summary> It's set to `{AreaName}_{ControllerName}_{ActionName}` </summary>
        public string ActionId { get; set; }

        /// <summary>  It's set to `{AreaName}/{ControllerName}/{ActionName}` </summary>
        public string ActionUrl { get; set; }

        /// <summary> Returns true if the action method has an `AuthorizeAttribute`. </summary>
        public bool IsSecuredAction { get; set; }

        /// <summary> Return `AreaAttribute.RouteValue` /// </summary>
        public string AreaName { get; set; }

        /// <summary> Return ControllerActionDescriptor.ControllerName </summary>
        public string ControllerName { get; set; }

        /// <summary>  Get name of controller from `{MvcTreeViewAttribute}` or return {AreaName} </summary>
        public Type ParentControllerType { get; set; }

        /// <summary> Type of current controller </summary>
        public Type ControllerType { get; set; }

        /// <summary>  It's set to `{AreaName}_{ControllerName}`/// </summary>
        public string ControllerId { get; set; }

        /// <summary> Returns the list of the ControllerType's Attributes or MethodInfo's Attributes. /// </summary>
        public IList<Attribute> Attributes { get; set; }

        /// <summary> Returns the list of the ControllerType's action methods. </summary>
        public IList<MvcTreeViewModel> Items { get; set; } =
            new List<MvcTreeViewModel>();


        /// <summary> Returns `[{controllerAttributes}]{AreaName}.{ControllerName}` </summary>
        public override string ToString()
        {
            const string attribute = "Attribute";
            var controllerAttributes = string.Join(",",
                Attributes.Select(a => a.GetType().Name.Replace(attribute, "")));
            return $"[{controllerAttributes}]{AreaName}.{ControllerName}";
        }
    }

    #endregion

    /// <summary> MvcActionItems Discovery Service Extensions </summary>
    public static class MvcDiscoveryServiceExtensions
    {
        /// <summary> Adds IMvcActionsDiscoveryService to IServiceCollection. </summary>
        public static IServiceCollection AddMvcDiscoveryService(this IServiceCollection services)
        {
            services.TryAddSingleton<IMvcDiscoveryService, MvcDiscoveryService>();
            return services;
        }
    }

    /// <summary> </summary>
    public interface IMvcDiscoveryService
    {
        /// <summary>
        /// Set or get a resource type
        /// </summary>
        IStringLocalizer Resource { get; set; }

        /// <summary> Returns the list of all of the controllers and action methods of an MVC application. </summary>
        ICollection<MvcTreeViewModel> MvcControllers { get; }

        /// <summary> return IReadOnly list of ControllerActionDescriptor </summary>
        /// <returns></returns>
        IEnumerable<ControllerActionDescriptor> GetControllerActionDescriptor { get; }

        /// <summary> Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.</summary>
        IEnumerable<MvcTreeViewModel> FilterSecureControllers(string policyName);

        /// <summary> Returns the list of all of the controllers and action methods of an MVC application which have Attribute.</summary>
        IEnumerable<MvcTreeViewModel> FilterControllersByAttribute<TAttribute>() where TAttribute : Attribute;

        /// <summary> Returns the list of all of the controllers and action methods of an MVC application which have Attribute.</summary>
        IEnumerable<MvcTreeViewModel> FilterActionsByAttribute<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        IEnumerable<MvcTreeViewModel> FilterControllerAndActionsByAttribute<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        IEnumerable<MvcTreeViewModel> FilterControllerOrActionsByAttribute<TAttribute>()
            where TAttribute : Attribute;

        /// <summary> Generate automatically a {MvcTreeView} from controllers and actions that has a {MvcTreeViewAttribute} feature </summary>
        /// <returns>MvcTreeViewModel</returns>
        MvcTreeViewModel BuildMvcTreeView<TAttribute>(MvcTreeViewModel rootNode)
            where TAttribute : MvcTreeViewAttribute;
    }


    /// <summary>MvcActionItems Discovery Service</summary>
    public class MvcDiscoveryService : IMvcDiscoveryService
    {
        private readonly IActionDescriptorCollectionProvider _actionsProvider;
        private readonly IMvcUrlService _mvcUrlService;

        /// <summary> You can set up a resource </summary>
        public IStringLocalizer Resource { get; set; }

        // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the GetterInfo more than
        // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
        // threads but only one of the objects succeeds in creating a GetterInfo.
        private readonly ConcurrentDictionary<string, Lazy<ICollection<MvcTreeViewModel>>>
            _mvcTreeViewItems = new ConcurrentDictionary<string, Lazy<ICollection<MvcTreeViewModel>>>();

        #region Constroctur

        /// <summary>
        /// MvcActionItems Discovery Service
        /// To use the {UrlService} you need to register it with the dependency service.
        /// To easy register can you use {WithMvcUrlService}extension method
        /// </summary>
        public MvcDiscoveryService(
            //IMvcUrlService mvcUrlService,
            IActionDescriptorCollectionProvider actionsProvider)
        {
            _actionsProvider = Ensure.IsNotNullWithDetails(actionsProvider, nameof(actionsProvider));
            //_mvcUrlService = mvcUrlService;
        }

        #endregion

        #region MvcControllerItems

        /// <summary> Returns the list of all of the controllers and action methods of an MVC application. </summary>
        public ICollection<MvcTreeViewModel> MvcControllers => PopulateMvcControllerViewModel();

        /// <summary> </summary>
        /// <returns></returns>
        public IEnumerable<ControllerActionDescriptor> GetControllerActionDescriptor
        {
            get
            {
                var actionDescriptors = _actionsProvider.ActionDescriptors.Items;

                foreach (var actionDescriptor in actionDescriptors)
                {
                    if (!(actionDescriptor is ControllerActionDescriptor descriptor))
                        continue;
                    yield return descriptor;
                }
            }
        }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        public IEnumerable<MvcTreeViewModel> FilterSecureControllers(string policyName)
        {
            var getter = _mvcTreeViewItems.GetOrAdd(policyName, y =>
                new Lazy<ICollection<MvcTreeViewModel>>(() =>
                {
                    var mvcTreeViewModels = new List<MvcTreeViewModel>(MvcControllers);
                    foreach (var mvcTreeViewModel in mvcTreeViewModels)
                    {
                        mvcTreeViewModel.Items = mvcTreeViewModel.Items
                            .Where(model =>
                                {
                                    var secureAction = model.Attributes?.OfType<AuthorizeAttribute>().FirstOrDefault();
                                    var secureController = mvcTreeViewModel.Attributes?.OfType<AuthorizeAttribute>()
                                        .FirstOrDefault();
                                    return model.IsSecuredAction &&
                                           (secureAction != null && secureAction.Policy == policyName ||
                                            secureController != null && secureController.Policy == policyName);
                                }
                            ).ToList();
                    }

                    return mvcTreeViewModels.Where(model => model.Items.Any()).ToList();
                }));
            return getter.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public IEnumerable<MvcTreeViewModel> FilterControllersByAttribute<TAttribute>() where TAttribute : Attribute
        {
            var fullName = typeof(TAttribute).FullName;
            return from controller in MvcControllers
                let controllerAttribute = controller.Attributes?.OfType<TAttribute>()
                    .FirstOrDefault(x => x.TypeId.ToString().Equals(fullName))
                where controllerAttribute != null
                select controller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public IEnumerable<MvcTreeViewModel> FilterActionsByAttribute<TAttribute>() where TAttribute : Attribute
        {
            var mvcTreeViewModels = new List<MvcTreeViewModel>(MvcControllers);
            var actions = new List<MvcTreeViewModel>();
            var fullName = typeof(TAttribute).FullName;
            foreach (var mvcTreeViewModel in mvcTreeViewModels)
            {
                var items = mvcTreeViewModel.Items
                    .Where(model =>
                        model.Attributes?.OfType<TAttribute>()
                            .FirstOrDefault(x => x.TypeId.ToString().Equals(fullName)) != null).ToList();
                actions.AddRange(items);
            }

            return actions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public IEnumerable<MvcTreeViewModel> FilterControllerAndActionsByAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var mvcTreeViewModels = new List<MvcTreeViewModel>();
            foreach (var mvcTreeViewModel in MvcControllers)
            {
                var fullName = typeof(TAttribute).FullName;

                var controllerAttribute = mvcTreeViewModel.Attributes?.OfType<TAttribute>()
                    .FirstOrDefault();

                if (controllerAttribute == null || !controllerAttribute.TypeId.ToString().Equals(fullName)) continue;
                mvcTreeViewModels.Add(mvcTreeViewModel);
                mvcTreeViewModels.AddRange(mvcTreeViewModel.Items
                    .Where(model =>
                    {
                        var attribute = model.Attributes?.OfType<TAttribute>()?.FirstOrDefault();
                        return attribute != null && attribute.TypeId.ToString().Equals(fullName);
                    }).ToList());
            }

            return mvcTreeViewModels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public IEnumerable<MvcTreeViewModel> FilterControllerOrActionsByAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var mvcTreeViewModels = new List<MvcTreeViewModel>(MvcControllers);
            var fullName = typeof(TAttribute).FullName;
            foreach (var mvcTreeViewModel in mvcTreeViewModels)
            {
                mvcTreeViewModel.Items = mvcTreeViewModel.Items
                    .Where(model =>
                    {
                        var controllerAttribute = mvcTreeViewModel.Attributes?.OfType<TAttribute>()
                            .FirstOrDefault(x => x.TypeId.ToString().Equals(fullName));
                        return model.Attributes?.OfType<TAttribute>()
                                   .FirstOrDefault(x => x.TypeId.ToString().Equals(fullName)) != null ||
                               controllerAttribute != null;
                    }).ToList();
            }

            return mvcTreeViewModels.Where(model => model.Items.Any());
        }

        #endregion

        /// <summary>
        /// Returns the MvcTreeView of all of the controllers and action methods of an MVC application, that have a {MvcTreeViewAttribute} actionAttribute
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MvcTreeViewModel BuildMvcTreeView<TAttribute>(MvcTreeViewModel rootNode)
            where TAttribute : MvcTreeViewAttribute
        {
            return FilterMvcTreeViewModels<TAttribute>()
                .ToList()
                .BuildMvcTreeView(rootNode);
        }

        #region Helper Methods

        /// <summary> With the number of action methods available, mvcTreeViewModel is generated </summary>
        private ICollection<MvcTreeViewModel> PopulateMvcControllerViewModel()
        {
            var mvcControllers = new List<MvcTreeViewModel>();

            var lastControllerName = string.Empty;

            MvcTreeViewModel mvcTreeViewModel = null;

            // loop flatted controllersActions
            // Controller -> Controller.Action1 -> Controller.Action2 ...
            foreach (var controllerAction in GetControllerActionDescriptor)
            {
                if (lastControllerName != controllerAction.ControllerName)
                {
                    var areaName = GetAreaName(controllerAction.ControllerTypeInfo);
                    mvcTreeViewModel = new MvcTreeViewModel
                    {
                        AreaName = areaName,
                        Attributes = GetAttributes(controllerAction.ControllerTypeInfo),
                        ControllerName = controllerAction.ControllerName,
                        ControllerType = controllerAction.ControllerTypeInfo,
                        ControllerId =
                            $"{areaName}{MvcConstant.UnderLineSeparator}{controllerAction.ControllerName}"
                    };

                    mvcControllers.Add(mvcTreeViewModel);
                    lastControllerName = controllerAction.ControllerName;
                }

                mvcTreeViewModel?.Items.Add(new MvcTreeViewModel
                {
                    AreaName = mvcTreeViewModel.AreaName,
                    ControllerId = mvcTreeViewModel.ControllerId,
                    ControllerType = mvcTreeViewModel.ControllerType,
                    ControllerName = mvcTreeViewModel.ControllerName,
                    Attributes = GetAttributes(controllerAction.MethodInfo),
                    ParentControllerType = mvcTreeViewModel.ControllerType,

                    ActionName = controllerAction.ActionName,
                    IsSecuredAction =
                        IsSecuredAction(controllerAction.ControllerTypeInfo, controllerAction.MethodInfo),
                    ActionUrl = CreateActionUrl(controllerAction),
                    ActionId =
                        $"{mvcTreeViewModel.ControllerId}{MvcConstant.UnderLineSeparator}{controllerAction.ActionName}"
                });
            }

            return mvcControllers;
        }

        private IEnumerable<MvcTreeViewModel> FilterMvcTreeViewModels<TAttribute>()
            where TAttribute : MvcTreeViewAttribute
        {
            var mvcTreeViewModels = MvcControllers;

            return mvcTreeViewModels.Select(mvcTreeViewModel =>
            {
                FilterControllerActionsByAttribute<TAttribute>(mvcTreeViewModel);
                return mvcTreeViewModel;
            });
        }

        private void FilterControllerActionsByAttribute<TAttribute>(MvcTreeViewModel mvcTreeViewModel)
            where TAttribute : MvcTreeViewAttribute
        {
            mvcTreeViewModel.CheckArgumentIsNull(nameof(mvcTreeViewModel));

            PopulateControllerFromAttribute<TAttribute>(mvcTreeViewModel);

            mvcTreeViewModel.Items = mvcTreeViewModel.Items
                .Where(x => x.ControllerId == mvcTreeViewModel.ControllerId)
                .Select(action =>
                {
                    PopulateNodeFromAction<TAttribute>(action);
                    return action;
                })
                .OrderBy(m => m.Order)
                .ToList();
        }


        private void PopulateControllerFromAttribute<TAttribute>(MvcTreeViewModel mvcTreeViewModel)
            where TAttribute : MvcTreeViewAttribute
        {
            var fullName = typeof(TAttribute).FullName;
            mvcTreeViewModel.CheckArgumentIsNull(nameof(mvcTreeViewModel));
            var controllerAttribute = mvcTreeViewModel.Attributes.OfType<TAttribute>()
                .FirstOrDefault(x => x.TypeId.ToString().Equals(fullName));
            if (controllerAttribute == null) return;


            var filters = controllerAttribute.FilterByAttributes?.Select(x => x.FullName).ToList();
            var attributes = mvcTreeViewModel.Attributes?.Select(x => x.GetType().FullName).ToList();
            if (filters != null)
            {
                var hasAll = attributes.ContainsAll(filters, p => p);
                if (!hasAll) return;
            }

            var authorize = mvcTreeViewModel.Attributes.OfType<AuthorizeAttribute>().FirstOrDefault();
            if (authorize != null && controllerAttribute.PolicyName.IsNotEmpty())
            {
                if (!authorize.Policy.Equals(controllerAttribute.PolicyName))
                    return;
            }

            mvcTreeViewModel.ParentControllerType = controllerAttribute.ParentControllerType;
            mvcTreeViewModel.Id = controllerAttribute.Id;
            mvcTreeViewModel.Title = controllerAttribute.Title.IsNotEmpty()
                ? Resource?[controllerAttribute.Title]
                : mvcTreeViewModel.ControllerName;
            mvcTreeViewModel.Order = controllerAttribute.Order;
            mvcTreeViewModel.CssIcon = controllerAttribute.CssIcon;
            mvcTreeViewModel.CssClass = controllerAttribute.CssClass;
            mvcTreeViewModel.IsVisible = controllerAttribute.Visible;
            mvcTreeViewModel.IsClickable = controllerAttribute.IsClickable;
            mvcTreeViewModel.Checked = controllerAttribute.Checked;
            mvcTreeViewModel.Description = controllerAttribute.Description;
            mvcTreeViewModel.Enabled = controllerAttribute.Enable;
            mvcTreeViewModel.Expanded = controllerAttribute.Expanded;
            mvcTreeViewModel.Selected = controllerAttribute.Selected;
            mvcTreeViewModel.ImageUrl = controllerAttribute.ImageUrl;
        }

        private void PopulateNodeFromAction<TAttribute>(MvcTreeViewModel mvcTreeViewModel)
            where TAttribute : MvcTreeViewAttribute
        {
            mvcTreeViewModel.CheckArgumentIsNull(nameof(mvcTreeViewModel));
            var fullName = typeof(TAttribute).FullName;
            var actionAttribute = mvcTreeViewModel.Attributes.OfType<TAttribute>()
                ?.FirstOrDefault(x => x.TypeId.ToString().Equals(fullName));
            if (actionAttribute == null) return;

            var filters = actionAttribute.FilterByAttributes?.Select(x => x.FullName).ToList();
            var attributes = mvcTreeViewModel.Attributes?.Select(x => x.GetType().FullName).ToList();
            if (filters != null)
            {
                var hasAll = attributes.ContainsAll(filters, p => p);
                if (!hasAll) return;
            }

            var authorize = mvcTreeViewModel.Attributes.OfType<AuthorizeAttribute>().FirstOrDefault();
            if (authorize != null && actionAttribute.PolicyName.IsNotEmpty())
            {
                if (!authorize.Policy.Equals(actionAttribute.PolicyName))
                    return;
            }

            mvcTreeViewModel.ParentControllerType = actionAttribute.ParentControllerType;
            mvcTreeViewModel.Id = actionAttribute.Id;
            mvcTreeViewModel.Title = actionAttribute.Title.IsNotEmpty()
                ? Resource?[actionAttribute.Title]
                : mvcTreeViewModel.ActionName;
            mvcTreeViewModel.Order = actionAttribute.Order;
            mvcTreeViewModel.CssIcon = actionAttribute.CssIcon;
            mvcTreeViewModel.CssClass = actionAttribute.CssClass;
            mvcTreeViewModel.IsVisible = actionAttribute.Visible;
            mvcTreeViewModel.IsClickable = actionAttribute.IsClickable;
            mvcTreeViewModel.Checked = actionAttribute.Checked;
            mvcTreeViewModel.Description = actionAttribute.Description;
            mvcTreeViewModel.Enabled = actionAttribute.Enable;
            mvcTreeViewModel.Expanded = actionAttribute.Expanded;
            mvcTreeViewModel.Selected = actionAttribute.Selected;
            mvcTreeViewModel.Url = mvcTreeViewModel.ActionUrl;
            mvcTreeViewModel.ImageUrl = actionAttribute.ImageUrl;
        }

        private static string GetAreaName(TypeInfo controllerTypeInfo)
        {
            return controllerTypeInfo?.GetCustomAttribute<AreaAttribute>()?.RouteValue ?? "";
        }

        private static List<Attribute> GetAttributes(MemberInfo actionMethodInfo)
        {
            return actionMethodInfo?.GetCustomAttributes(inherit: true)
                .Where(attribute =>
                {
                    var attributeNamespace = attribute.GetType().Namespace;
                    return attributeNamespace != typeof(CompilerGeneratedAttribute).Namespace &&
                           attributeNamespace != typeof(DebuggerStepThroughAttribute).Namespace;
                })
                .Cast<Attribute>()
                .Distinct()
                .ToList();
        }

        private string CreateActionUrl(ControllerActionDescriptor controllerAction)
        {
            //if (_mvcUrlService != null)
            //{
            //    return _mvcUrlService?.Url?.Action(
            //        controllerAction?.ActionName,
            //        controllerAction?.ControllerName,
            //        new { area = GetAreaName(controllerAction?.ControllerTypeInfo) });
            //}

            return
                $"{GetAreaName(controllerAction?.ControllerTypeInfo)}{MvcConstant.SlashSeparator}{controllerAction?.ControllerName}{MvcConstant.SlashSeparator}{controllerAction?.ActionName}";
        }

        private static bool IsSecuredAction(TypeInfo controllerType, MethodInfo actionMethodInfo)
        {
            if (HasAnonymousAttribute(actionMethodInfo)) return false;

            var controllerHasAuthorizeAttribute =
                controllerType?.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            if (controllerHasAuthorizeAttribute)
            {
                return true;
            }

            var actionMethodHasAuthorizeAttribute =
                actionMethodInfo?.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            return actionMethodHasAuthorizeAttribute;
        }

        private static bool HasAnonymousAttribute(MethodInfo actionMethodInfo)
        {
            return actionMethodInfo?.GetCustomAttribute<AllowAnonymousAttribute>(inherit: true) != null;
        }

        private static bool IsSecuredAction(MethodInfo methodInfo)
        {
            var actionHasAllowAnonymousAttribute =
                methodInfo?.GetCustomAttribute<AllowAnonymousAttribute>(inherit: true) != null;
            if (actionHasAllowAnonymousAttribute)
            {
                return false;
            }

            var actionMethodHasAuthorizeAttribute =
                methodInfo?.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            return actionMethodHasAuthorizeAttribute;
        }

        #endregion
    }

    internal static class Extensions
    {
        public static bool ContainsAll<T, TKey>(this IEnumerable<T> list1, IEnumerable<T> list2, Func<T, TKey> key)
        {
            var containingList = new HashSet<TKey>(list1.Select(key));
            return list2.All(x => containingList.Contains(key(x)));
        }

        public static bool ContainsAll<T>(this IEnumerable<T> list1, IEnumerable<T> list2) =>
            list1.ContainsAll(list2, item => item);
    }
}