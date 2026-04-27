using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceCraft
{
    public class InventoryAuthorizedItems : InventoryAuthorizedGroups
    {
        [SerializeField]
        List<GroupData> _authorizedItems;
        public void Start()
        {
            if (_authorizedItems == null) { return; }
            FieldInfo FieldInfo_InventoryAuthorizedGroups__authorizedGroups = typeof(InventoryAuthorizedGroups).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(e => e.Name.Contains("_authorizedGroups")).First();
            object authGroupsAsObj = FieldInfo_InventoryAuthorizedGroups__authorizedGroups.GetValue((InventoryAuthorizedGroups)this);
            HashSet<Group> _authorizedGroups;
            if (authGroupsAsObj == null)
            {
                _authorizedGroups = new HashSet<Group>();
            }
            else
            {
                _authorizedGroups = (HashSet<Group>)authGroupsAsObj;
            }
            foreach (var gd in _authorizedItems)
            {
                _authorizedGroups.Add(GroupsHandler.GetGroupViaId(gd.id));
            }
            FieldInfo_InventoryAuthorizedGroups__authorizedGroups.SetValue(this, _authorizedGroups);
        }
    }
}
