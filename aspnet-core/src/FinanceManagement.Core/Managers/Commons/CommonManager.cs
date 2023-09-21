using Abp.Domain.Entities;
using Abp.Linq.Extensions;
using DocumentFormat.OpenXml.Wordprocessing;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Commons
{
    public class CommonManager : DomainManager, ICommonManager
    {
        public CommonManager(IWorkScope ws) : base(ws)
        {
        }

        public bool IsOutcomingEntryNameExist(string inputName)
        {
            return _ws.GetAll<OutcomingEntry>()
                .Any(x => x.Name == inputName);
        }

        public async Task<long> GetDefaultAccountCompanyId()
        {
            return await _ws.GetAll<Account>()
                .Where(x => x.Type == AccountTypeEnum.COMPANY && x.Default == true)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetOutcomingEntryTypeSalaryId()
        {
            var maLoaiChiBangLuong = await MySettingManager.GetMaLoaiChiBangLuong();
            return await _ws.GetAll<OutcomingEntryType>()
                .Where(x => x.Code.ToLower() == maLoaiChiBangLuong.ToLower())
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetOutcomingEntryTypeTeamBuildingId()
        {
            return await _ws.GetAll<OutcomingEntryType>()
                .Where(x => x.Code == FinanceManagementConsts.OUTCOMING_ENTRY_TYPE_TEAM_BUILDING)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetWorkflowStatusApprovedId()
        {
            return await _ws.GetAll<WorkflowStatus>()
                .Where(x => x.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetWorkflowStatusStartId()
        {
            return await _ws.GetAll<WorkflowStatus>()
                .Where(x => x.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_START)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public long GetWorkflowStatusENDId()
        {
            return _ws.GetAll<WorkflowStatus>()
                .Where(x => x.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_END)
                .Select(x => x.Id)
                .FirstOrDefault();
        }

        public async Task<long> GetCurrencyVNDId()
        {
            return await _ws.GetAll<Currency>()
                .Where(x => x.Name == FinanceManagementConsts.VND_CURRENCY_NAME)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }
        public IEnumerable<TreeItem<OutputCategoryEntryType>> GetTreeIncomingEntries(bool isActiveOnly = true)
        {
            var incomings = _ws.GetAll<IncomingEntryType>()
                .WhereIf(isActiveOnly == true, s => s.IsActive)
                .Select(s => new OutputCategoryEntryType
                {
                    Id = s.Id,
                    Name = s.Name,
                    ParentId = s.ParentId,
                    IsActive = s.IsActive
                })
                .ToList();
            var result = incomings.GenerateTree(c => c.Id, c => c.ParentId);
            return result;
        }
        /// <summary>
        /// false: là lấy hết, true: chỉ lấy isActive
        /// </summary>
        /// <param name="isActiveOnly"></param>
        /// <returns></returns>
        public IEnumerable<TreeItem<OutputCategoryEntryType>> GetTreeOutcomingEntries(bool isActiveOnly = true)
        {
            var outcomings = _ws.GetAll<OutcomingEntryType>()
                .WhereIf(isActiveOnly == true, s => s.IsActive)
                .Select(s => new OutputCategoryEntryType
                {
                    Id = s.Id,
                    Name = s.Name,
                    ParentId = s.ParentId,
                    IsActive = s.IsActive
                })
                .ToList();
            var result = outcomings.GenerateTree(c => c.Id, c => c.ParentId);
            return result;
        }
        public TreeItem<OutputCategoryEntryType> GetTreeEntryWithRoot(IEnumerable<OutputCategoryEntryType> list)
        {
            var tree = new TreeItem<OutputCategoryEntryType>();
            tree.Item = new OutputCategoryEntryType();
            tree.Children = list.GenerateTree(c => c.Id, c => c.ParentId);
            return tree;
        }
        public TreeItem<OutputCategoryEntryType> GetTreeOutcomingEntryWithRoot(bool isActive)
        {
            var newTree = new TreeItem<OutputCategoryEntryType>();
            newTree.Item = new OutputCategoryEntryType();
            newTree.Children = GetTreeOutcomingEntries(isActive);
            return newTree;
        }
        public TreeItem<OutputCategoryEntryType> GetTreeIncomingEntryWithRoot(bool isActive)
        {
            var newTree = new TreeItem<OutputCategoryEntryType>();
            
            newTree.Item = new OutputCategoryEntryType();
            newTree.Children = GetTreeIncomingEntries(isActive);

            return newTree;
        }
        public TreeItem<OutputCategoryEntryType> GetTreeEntryWithRoot(IEnumerable<TreeItem<OutputCategoryEntryType>> tree)
        {
            var newTree = new TreeItem<OutputCategoryEntryType>();

            newTree.Item = new OutputCategoryEntryType();
            newTree.Children = tree;

            return newTree;
        }
        public List<long> GetAllEntryUpperNodeIds(long nodeId, TreeItem<OutputCategoryEntryType> tree)
        {
            var list = new List<long>();
            GetUpperNodeId(tree, nodeId, list);
            return list;
        }
        private long? GetUpperNodeId(TreeItem<OutputCategoryEntryType> node, long findNodeId, List<long> result)
        {
            if (node.Item.Id == findNodeId)
            {
                return node.Item.Id;
            }
            foreach (var item in node.Children)
            {
                var parentId = GetUpperNodeId(item, findNodeId, result);
                if(parentId != null)
                {
                    result.Add(item.Item.Id);
                    return item.Item.Id;
                }
            }
            return null;
        }
        public HashSet<long> GetAllNodeAndLeafEntryTypeIdByParentId<T>(IEnumerable<long> selectedNodeIds, bool isInActive = false) where T : class, IEntity<long>
        {
            IEnumerable<TreeItem<OutputCategoryEntryType>> tree;
            if (typeof(T) == typeof(OutcomingEntryType))
            {
                tree = GetTreeOutcomingEntries(isInActive);
            }
            else if (typeof(T) == typeof(IncomingEntryType))
            {
                tree = GetTreeIncomingEntries(isInActive);
            }
            else
            {
                return new HashSet<long>();
            }

            var listIds = new HashSet<long>();
            GetEntryTypeIdsFromTree(selectedNodeIds, tree, false, listIds);
            return listIds;
        }
        public void GetEntryTypeIdsFromTree(
            IEnumerable<long> selectedNodeIds,
            IEnumerable<TreeItem<OutputCategoryEntryType>> findInListNode,
            bool isAccepted,
            HashSet<long> resultListIds
        )
        {
            foreach (var node in findInListNode)
            {
                if (selectedNodeIds.Contains(node.Item.Id) || isAccepted)
                {
                    resultListIds.Add(node.Item.Id);
                    if (node.Children != null && node.Children.Any())
                    {
                        GetEntryTypeIdsFromTree(selectedNodeIds, node.Children, true, resultListIds);
                    }
                }
                GetEntryTypeIdsFromTree(selectedNodeIds, node.Children, false, resultListIds);
            }
        }
        public void GetLowerEntryTypeIdsFromTreeWithRoot(
            long selectedNodeId,
            TreeItem<OutputCategoryEntryType> root,
            bool isAccepted,
            List<long> resultListIds
        )
        {
            foreach (var node in root.Children)
            {
                if (selectedNodeId == node.Item.Id || isAccepted)
                {
                    resultListIds.Add(node.Item.Id);
                    if (node.Children != null && node.Children.Any())
                    {
                        GetLowerEntryTypeIdsFromTreeWithRoot(selectedNodeId, node, true, resultListIds);
                    }
                }
                GetLowerEntryTypeIdsFromTreeWithRoot(selectedNodeId, node, false, resultListIds);
            }
        }
        public List<long> GetAllEntryLowerNodeIds(long selectedNodeId, TreeItem<OutputCategoryEntryType> tree)
        {
            var resultListIds = new List<long>();
            GetLowerEntryTypeIdsFromTreeWithRoot(selectedNodeId, tree, false, resultListIds);
            return resultListIds;
        }
        public HashSet<long> GetNodeIdsFromTree(long parentId, IEnumerable<TreeItem<OutputCategoryEntryType>> findInListNode)
        {
            var resultSet = new HashSet<long>();
            GetEntryTypeIdsFromTree(new List<long> { parentId }, findInListNode, false, resultSet);
            return resultSet;
        }
        public async Task<long> GetStatusIdByCode(string code)
        {
            return await _ws.GetAll<WorkflowStatus>()
                .Where(x => x.Code == code)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetTreeOutcomingTypeByUserId(long userId, bool isShowAll)
        {
            var listTypes = await _ws.GetAll<UserOutcomingType>().Where(uot => uot.UserId == userId)
                                  .Select(x => x.OutcomingEntryTypeId).ToListAsync();

            var incomings = await _ws.GetAll<OutcomingEntryType>()
                .Where(oet => listTypes.Contains(oet.Id))
                .WhereIf(!isShowAll, s => s.IsActive)
                .Select(s => new OutputCategoryEntryType
                {
                    Id = s.Id,
                    Name = s.Name,
                    ParentId = s.ParentId,
                    IsActive = s.IsActive
                })
                .ToListAsync();
            var result = incomings.GenerateTree(c => c.Id, c => c.ParentId);
            return result;
        }
    }
}
