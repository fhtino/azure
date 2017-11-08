echo --- START ---
myPubIP=$(curl https://ifconfig.co)
read -p "The publis IP address is:  $myPubIP   Press enter to continue"
echo "Updating NSG rule. Please wait..."
az network nsg rule update \
   --nsg-name "my_nsg_rule_name" \
   --resource-group "my_res_group" \
   --name "SSH_FromCloudShell" \
   --source-address-prefix $myPubIP \
   --access Allow
echo --- THE END ---