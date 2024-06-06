--SELECT 

--'9' as "INV12 Incoterms",
--T1."ItemCode" ,
--SUM(T1."Quantity") as "Quantity",
--SUM(T1."StockPrice") as "Price",
--CASE 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN '26-PRPAS' 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN '26-PRSEL' 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN '26-ARM01' 
--WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN '10-ARM01' 
--END as "WarehouseCode",
--(select "U_UtilizSai" from "@K33P_TRAN_PADC" WHERE "U_FilialSai" = 
--CASE 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 6 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 6
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 6
--WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 11
--END ) as "Usage",

--T1."FromWhsCod" as "Origem",
--T1."WhsCode" as "Destino"

--FROM OWTR T0 
--INNER JOIN WTR1 T1 ON T0."DocEntry" = T1."DocEntry"
--INNER JOIN OWTQ TQ ON CAST(TQ."DocNum" as nvarchar) = T1."BaseRef"

--WHERE 
----T1."BaseRef" IN (SELECT CAST("DocNum" as nvarchar) FROM OWTQ WHERE "CANCELED" = 'N' AND "DocStatus" = 'C')
--T0."CANCELED" = 'N'
--AND TQ."CANCELED" = 'N' 
--AND TQ."DocStatus" = 'C'
--AND T1."FromWhsCod" IN ('10-ARM01','26-ARM01')--colocar no config essa string
--AND T1."WhsCode" IN ('10-PRPAS','10-PRSEL','10-26AR1','26-10AR1')--colocar no config essa string
--AND IFNULL(T0."U_ImportNFE",'N') = 'N'
--AND TQ."DocNum" = {0}

--GROUP BY 
--T1."BaseRef",
--T1."ItemCode",
--T1."FromWhsCod" ,
--T1."WhsCode" ,
--T0."DocNum" 

----UNION ALL

------------------------------------******* Transferencia nao atreladas a pedidos de tr.----------------------------------------------

----SELECT 

----'9' as "INV12 Incoterms",
----T1."ItemCode" ,
----SUM(T1."Quantity") as "Quantity",
----SUM(T1."StockPrice") as "Price",
----CASE 
----WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN '26-PRPAS' 
----WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN '26-PRSEL' 
----WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN '26-ARM01' 
----WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN '10-ARM01' 
----END as "WarehouseCode",
----(select "U_UtilizSai" from "@K33P_TRAN_PADC" WHERE "U_FilialSai" = 
----CASE 
----WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 6 
----WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 6
----WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 6
----WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 11
----END ) as "Usage",

----T1."FromWhsCod" as "Origem",
----T1."WhsCode" as "Destino"

----FROM OWTR T0 
----INNER JOIN WTR1 T1 ON T0."DocEntry" = T1."DocEntry"
------INNER JOIN OWTQ TQ ON CAST(TQ."DocNum" as nvarchar) = T1."BaseRef"

----WHERE 
----IFNULL(T1."BaseRef",'') = ''
------T1."BaseRef" NOT IN (SELECT CAST("DocNum" as nvarchar) FROM OWTQ WHERE "CANCELED" = 'N' AND "DocStatus" = 'C')
----AND T0."CANCELED" = 'N'
------AND TQ."CANCELED" = 'N' 
------AND TQ."DocStatus" = 'C'
----AND T1."FromWhsCod" IN ('10-ARM01','26-ARM01')--colocar no config essa string
----AND T1."WhsCode" IN ('10-PRPAS','10-PRSEL','10-26AR1','26-10AR1')--colocar no config essa string
----AND IFNULL(T0."U_ImportNFE",'N') = 'N'
----AND T0."DocNum" = {0}

----GROUP BY 
----T1."BaseRef",
----T1."ItemCode",
----T1."FromWhsCod" ,
----T1."WhsCode" ,
----T0."DocNum"

----ORDER BY 1,2
----;


--//////////////////////////////////////////VIEW/////////////////////////////////////////////////////////////////
SELECT * FROM K33P_TRANS_NF_ITEM_ENTRADA WHERE "Tipo" = 'Transf.'  AND "DocNum" = {0};