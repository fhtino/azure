date

mydt=$(date '+%Y%m%d-%H%M%S')

myserviceprincipal='http://...'
myserviceprincipalpass='...'
myserviceprincipaltenant='...'

myresourcegroup='...'
mysqlservername='...'
mysqlusername='...'
mysqlpassword='...'
mydatabase='...'
mytempdatabase='tempDBbackup...'

mystorageaccount='...'
mystoragekey='...'
mystoragecontainer='...'
mystoragefolder=$(date '+%Y-%m')
mystoragefilename='MyDB..._'$mydt'.bacpac'

mylocalfolder='/..../..../'


echo folder: $mystoragefolder
echo file:   $mystoragefilename


echo ---------- Login...
date
az login --service-principal -u "$myserviceprincipal" -p "$myserviceprincipalpass" --tenant "$myserviceprincipaltenant"


echo ---------- Delete MyTempDB
date
az sql db delete \
		--resource-group $myresourcegroup \
		--server $mysqlservername \
		--name $mytempdatabase \
		--yes


echo ---------- Copy DB to MyTempDB
date
az sql db copy --resource-group $myresourcegroup --server $mysqlservername --name $mydatabase --dest-name $mytempdatabase
	

echo ---------- Export MyTempDB to .bacpac
date
az sql db export \
        --server $mysqlservername \
        --resource-group $myresourcegroup  \
        --name $mytempdatabase \
        --storage-uri https://$mystorageaccount.blob.core.windows.net/$mystoragecontainer/$mystoragefolder/$mystoragefilename  \
        --storage-key $mystoragekey \
        --storage-key-type StorageAccessKey \
        --admin-user $mysqlusername \
        --admin-password $mysqlpassword
		
		
echo ---------- Delete MyTempDB
date
az sql db delete \
		--resource-group $myresourcegroup \
		--server $mysqlservername \
		--name $mytempdatabase \
		--yes

		
echo ---------- Copy  bacpac to local_sqlbackup
date
az storage blob download \
		--container-name $mystoragecontainer \
		--name $mystoragefolder/$mystoragefilename \
		--file $mylocalfolder/$mystoragefilename \
		--account-name $mystorageaccount

		
echo ---------- The End...
date
az logout

