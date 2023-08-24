using Abp.Dependency;
using Abp.Domain.Entities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Commons
{
    public interface ICommonManager : ITransientDependency
    {
        Task<long> GetWorkflowStatusApprovedId();
        Task<long> GetOutcomingEntryTypeSalaryId();
        Task<long> GetDefaultAccountCompanyId();
        Task<long> GetCurrencyVNDId();
        HashSet<long> GetAllNodeAndLeafEntryTypeIdByParentId<T>(IEnumerable<long> nodeIds, bool isInActive = false) where T : class, IEntity<long>;
        IEnumerable<TreeItem<OutputCategoryEntryType>> GetTreeOutcomingEntries(bool isActiveOnly = false);
        IEnumerable<TreeItem<OutputCategoryEntryType>> GetTreeIncomingEntries(bool isActiveOnly = false);
        void GetEntryTypeIdsFromTree(IEnumerable<long> selectedNodeIds, IEnumerable<TreeItem<OutputCategoryEntryType>> findInListNode, bool isAccepted, HashSet<long> resultListIds);
        HashSet<long> GetNodeIdsFromTree(long parentId, IEnumerable<TreeItem<OutputCategoryEntryType>> findInListNode);
        Task<long> GetStatusIdByCode(string code);
        List<long> GetAllEntryUpperNodeIds(long nodeId, TreeItem<OutputCategoryEntryType> tree);
        TreeItem<OutputCategoryEntryType> GetTreeEntryWithRoot(IEnumerable<TreeItem<OutputCategoryEntryType>> tree);
        TreeItem<OutputCategoryEntryType> GetTreeIncomingEntryWithRoot(bool isActive);
        TreeItem<OutputCategoryEntryType> GetTreeEntryWithRoot(IEnumerable<OutputCategoryEntryType> list);
        TreeItem<OutputCategoryEntryType> GetTreeOutcomingEntryWithRoot(bool isActive);
        List<long> GetAllEntryLowerNodeIds(long selectedNodeId, TreeItem<OutputCategoryEntryType> tree);
        Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetTreeOutcomingTypeByUserId(long userId, bool isShowAll);
        bool IsOutcomingEntryNameExist(string name);
        Task<long> GetWorkflowStatusStartId();
        Task<long> GetOutcomingEntryTypeTeamBuildingId();
    }
}
