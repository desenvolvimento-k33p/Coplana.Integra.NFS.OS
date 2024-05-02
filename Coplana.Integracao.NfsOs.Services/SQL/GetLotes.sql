select 

TOP 1 

"DistNumber" as "BatchNumber",
"SysNumber" as "SystemSerialNumber"

from OBTN 

WHERE 

"ItemCode" ='{0}'
AND "Status"  = 0
AND CURRENT_DATE <= "ExpDate"
AND "MnfDate" = (SELECT MIN("MnfDate") FROM OBTN WHERE "ItemCode" = '{0}' AND "Status"  = 0 AND CURRENT_DATE <= "ExpDate")