--select 

--TOP 1 

--"DistNumber" as "BatchNumber",
--"SysNumber" as "SystemSerialNumber"

--from OBTN 

--WHERE 

--"ItemCode" ='{0}'
--AND "Status"  = 0
--AND CURRENT_DATE <= "ExpDate"
--AND "MnfDate" = (SELECT MIN("MnfDate") FROM OBTN WHERE "ItemCode" = '{0}' AND "Status"  = 0 AND CURRENT_DATE <= "ExpDate")

SELECT 
IBT1."BatchNum" as "BatchNumber",
(SELECT "SysNumber" FROM OBTN WHERE  "ItemCode" ='{1}' AND "DistNumber" = IBT1."BatchNum" ) as "SystemSerialNumber"

FROM WTR1
INNER JOIN OITM 
ON WTR1."ItemCode" = OITM."ItemCode" 
INNER JOIN OWTR 
ON WTR1."DocEntry" = OWTR."DocEntry"
INNER JOIN IBT1 
ON WTR1."ItemCode" = IBT1."ItemCode" 
AND WTR1."WhsCode"  = IBT1."WhsCode" 
AND WTR1."ObjType" = IBT1."BaseType" 
AND WTR1."DocEntry" = IBT1."BaseEntry" 
AND WTR1."LineNum" = IBT1."BaseLinNum" 

WHERE 
OWTR."DocEntry" = {0}--138
AND WTR1."ItemCode" = '{1}'--'PA000000001'