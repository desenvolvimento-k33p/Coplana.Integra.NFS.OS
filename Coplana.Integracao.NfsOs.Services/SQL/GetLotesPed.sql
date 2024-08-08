--SELECT 
--IBT1."Quantity",
--IBT1."BatchNum" as "BatchNumber",
--(SELECT "SysNumber" FROM OBTN WHERE  "ItemCode" ='{1}' AND "DistNumber" = IBT1."BatchNum" ) as "SystemSerialNumber"

--FROM WTR1
--INNER JOIN OITM 
--ON WTR1."ItemCode" = OITM."ItemCode" 
--INNER JOIN OWTR 
--ON WTR1."DocEntry" = OWTR."DocEntry"
--INNER JOIN IBT1 
--ON WTR1."ItemCode" = IBT1."ItemCode" 
--AND WTR1."WhsCode"  = IBT1."WhsCode" 
--AND WTR1."ObjType" = IBT1."BaseType" 
--AND WTR1."DocEntry" = IBT1."BaseEntry" 
--AND WTR1."LineNum" = IBT1."BaseLinNum" 

--WHERE 
--WTR1."BaseRef" = '{0}'
--AND WTR1."ItemCode" = '{1}'

/* Com Pedido de Transferencia */
SELECT *
FROM K33P_TRANS_NF_LOTE_ENTRADA  x
WHERE
	x."BaseType" = 1250000001
AND x."BaseRef" = '{0}'
AND x."LineNum" = '{1}'	