SELECT distinct

IFNULL(T0."DocNum",0) as "Número Documento",
'Pedido de Transf.' as "Tipo Transf.",
T1."FromWhsCod" as "Do Depósito",
T1."WhsCode" as "Para Depósito",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' THEN 10 
WHEN T1."FromWhsCod" = '10-ARM01' THEN 10
WHEN T1."FromWhsCod" = '10-ARM01' THEN 10
WHEN T1."FromWhsCod" = '26-ARM01' THEN 26
END as "Da Filial",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 26
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 26
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 26
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 10 
END as "Para Filial",
(SELECT max("DocNum") FROm OINV WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar)) as "DocNum NF Saída",
(SELECT max("DocNum") FROm OPCH WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar)) as "DocNum NF Entrada"
,CASE WHEN DB."StatusId" = 4 then 'Autorizado o uso da Nfe'
	  WHEN DB."StatusId" <> 4 then
(SELECT "Hist" FROM "DBInvOne"."ProcessHist" WHERE "BatchId" = DB."BatchId" and "Id" = (SELECT MAX("Id") FROM "DBInvOne"."ProcessHist" WHERE "BatchId" = DB."BatchId" ))
END

 AS "STATUS_NFE"


FROM OWTR T0 
INNER JOIN WTR1 T1 ON T0."DocEntry" = T1."DocEntry"
INNER JOIN OWTQ TQ ON CAST(TQ."DocNum" as nvarchar) = T1."BaseRef"
LEFT JOIN "DBInvOne"."Process" DB ON DB."DocEntry" = (select MAX("DocEntry") from OINV WHERE "U_NumTransf" = T0."DocNum") AND DB."DocType" = 13
AND DB."CompanyId" = CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 51
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 51
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 51
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 56 
END


WHERE 

T0."CANCELED" = 'N'
AND TQ."CANCELED" = 'N' 
AND TQ."DocStatus" = 'C'
AND T1."FromWhsCod" IN ('10-ARM01','26-ARM01')
AND T1."WhsCode" IN ('10-PRPAS','10-PRSEL','10-26AR1','26-10AR1')

GROUP BY 
T1."BaseRef",
T1."ItemCode",
T1."FromWhsCod" ,
T1."WhsCode" ,
TQ."DocNum",
T0."DocNum",DB."BatchId",DB."StatusId"
--V."DocNum",
--P."DocNum"




UNION ALL

--------------------------------******* Transferencia nao atreladas a pedidos de tr.----------------------------------------------

SELECT distinct

IFNULL(T0."DocNum",0) as "Número Documento",
'Transf. sem Pedido' as "Tipo Transf.",
T1."FromWhsCod" as "Do Depósito",
T1."WhsCode" as "Para Depósito",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' THEN 10 
WHEN T1."FromWhsCod" = '10-ARM01' THEN 10
WHEN T1."FromWhsCod" = '10-ARM01' THEN 10
WHEN T1."FromWhsCod" = '26-ARM01' THEN 26
END as "Da Filial",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 26
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 26
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 26
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 10 
END as "Para Filial",
(SELECT max("DocNum") FROm OINV WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar)) as "DocNum NF Saída",
(SELECT max("DocNum") FROm OPCH WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar)) as "DocNum NF Entrada"

,CASE WHEN DB."StatusId" = 4 then 'Autorizado o uso da Nfe'
	  WHEN DB."StatusId" <> 4 then
(SELECT "Hist" FROM "DBInvOne"."ProcessHist" WHERE "BatchId" = DB."BatchId" and "Id" = (SELECT MAX("Id") FROM "DBInvOne"."ProcessHist" WHERE "BatchId" = DB."BatchId" ))
END

 AS "STATUS_NFE"

FROM OWTR T0 
INNER JOIN WTR1 T1 ON T0."DocEntry" = T1."DocEntry"
LEFT JOIN "DBInvOne"."Process" DB ON DB."DocEntry" = (select MAX("DocEntry") from OINV WHERE "U_NumTransf" = T0."DocNum") AND DB."DocType" = 13
AND DB."CompanyId" = CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 51
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 51
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 51
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 56 
END

WHERE 
IFNULL(T1."BaseRef",'') = ''
AND T0."CANCELED" = 'N'
--AND TQ."CANCELED" = 'N' 
AND T1."FromWhsCod" IN ('10-ARM01','26-ARM01')
AND T1."WhsCode" IN ('10-PRPAS','10-PRSEL','10-26AR1','26-10AR1')


GROUP BY 
T1."BaseRef",
T1."ItemCode",
T1."FromWhsCod" ,
T1."WhsCode" ,
T0."DocNum",DB."BatchId",DB."StatusId"

ORDER BY 1,2;



