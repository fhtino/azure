echo --- START ---
echo "Updating NSG rule. Please wait..."
az network nsg rule update \
   --nsg-name "my_nsg_rule_name" \
   --resource-group "my_res_group" \
   --name "SSH_FromCloudShell" \
   --access Deny
echo --- THE END ---