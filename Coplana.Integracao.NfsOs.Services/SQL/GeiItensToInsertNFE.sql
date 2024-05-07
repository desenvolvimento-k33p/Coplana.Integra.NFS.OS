SELECT DISTINCT
IFNULL(T1."BaseRef",'') as "DocNumPedTransf",
T0."DocNum" as "DocNumTransf",
T0."DocEntry" as "DocEntryTransf",
(select "U_PNEntrada" from "@K33P_TRAN_PADC" WHERE "U_FilialEnt" = 
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 11 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 11
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 11
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 6
END ) as "CardCode",
CURRENT_DATE as "DocDate",
CURRENT_DATE as "TaxDate",
CURRENT_DATE as "DocDueDate",
'E' as "U_K_TipoOrdem",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 11
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 11
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 11
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 6 
END as "BPL_IDAssignedToInvoice",
--(SELECT "SeqCode" FROM NFN1 WHERE "BPLId" = CASE 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 11
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 11
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 11
--WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 6 
--END) as "SequenceCode",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 37
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 37
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 37
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 32 
END as "SequenceCode",

T1."FromWhsCod" as "Origem",
T1."WhsCode" as "Destino",

'Com Pedido' as "Tipo"

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
--T1."BaseRef" IN (SELECT CAST("DocNum" as nvarchar) FROM OWTQ WHERE "CANCELED" = 'N' AND "DocStatus" = 'C')
T0."CANCELED" = 'N'
AND TQ."CANCELED" = 'N' 
AND TQ."DocStatus" = 'C'
AND T1."FromWhsCod" IN ('10-ARM01','26-ARM01')--colocar no config essa string
AND T1."WhsCode" IN ('10-PRPAS','10-PRSEL','10-26AR1','26-10AR1')--colocar no config essa string
AND IFNULL(T0."U_ImportNFE",'N') = 'N'
AND DB."StatusId" = 4
--(SELECT

--"S"."Description" AS "STATUS"
--FROM
--"DBInvOne"."Process" AS "P"
--INNER JOIN "DBInvOne"."ProcessHist" AS "H" ON
--"P"."BatchId" = "H"."BatchId"
--LEFT JOIN "DBInvOne"."ProcessStatus" AS "S" ON
--"S"."ID" = "P"."StatusId"
--WHERE "P"."DocEntry" = (SELECT "DocEntry" FROm OINV WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar))
--AND "P"."DocType" = 13
--AND "P"."CompanyId" = (SELECT "BPLId" FROm OINV WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar))) = 4

GROUP BY 
T1."BaseRef",
T1."ItemCode",
T1."FromWhsCod" ,
T1."WhsCode" ,
T0."DocNum",
T0."DocEntry"



UNION ALL

--------------------------------******* Transferencia nao atreladas a pedidos de tr.----------------------------------------------

SELECT DISTINCT
IFNULL(T1."BaseRef",'') as "DocNumPedTransf",
T0."DocNum" as "DocNum Transf",
T0."DocEntry" as "DocEntryTransf",

(select "U_PNEntrada" from "@K33P_TRAN_PADC" WHERE "U_FilialEnt" = 
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 11 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 11
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 11
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 6
END ) as "CardCode",
CURRENT_DATE as "DocDate",
CURRENT_DATE as "TaxDate",
CURRENT_DATE as "DocDueDate",
'E' as "U_K_TipoOrdem",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 11
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 11
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 11
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 6 
END as "BPL_IDAssignedToInvoice",
--(SELECT "SeqCode" FROM NFN1 WHERE "BPLId" = CASE 
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 11
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 11
--WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 11
--WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 6 
--END) as "SequenceCode",
CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 37
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 37
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 37
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 32 
END as "SequenceCode",

T1."FromWhsCod" as "Origem",
T1."WhsCode" as "Destino",
'Sem Pedido' as "Tipo"

FROM OWTR T0 
INNER JOIN WTR1 T1 ON T0."DocEntry" = T1."DocEntry"
LEFT JOIN "DBInvOne"."Process" DB ON DB."DocEntry" = (select MAX("DocEntry") from OINV WHERE "U_NumTransf" = T0."DocNum") AND DB."DocType" = 13
AND DB."CompanyId" = CASE 
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRPAS' THEN 51
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-PRSEL' THEN 51
WHEN T1."FromWhsCod" = '10-ARM01' AND T1."WhsCode" = '10-26AR1' THEN 51
WHEN T1."FromWhsCod" = '26-ARM01' AND T1."WhsCode" = '26-10AR1' THEN 56 
END
--INNER JOIN OWTQ TQ ON CAST(TQ."DocNum" as nvarchar) = T1."BaseRef"

WHERE 
IFNULL(T1."BaseRef",'') = ''
--T1."BaseRef" NOT IN (SELECT CAST("DocNum" as nvarchar) FROM OWTQ WHERE "CANCELED" = 'N' AND "DocStatus" = 'C')
AND T0."CANCELED" = 'N'
--AND TQ."CANCELED" = 'N' 
--AND TQ."DocStatus" = 'C'
AND T1."FromWhsCod" IN ('10-ARM01','26-ARM01')--colocar no config essa string
AND T1."WhsCode" IN ('10-PRPAS','10-PRSEL','10-26AR1','26-10AR1')--colocar no config essa string
AND IFNULL(T0."U_ImportNFE",'N') = 'N'
AND DB."StatusId" = 4
--(SELECT

--"S"."Description" AS "STATUS"
--FROM
--"DBInvOne"."Process" AS "P"
--INNER JOIN "DBInvOne"."ProcessHist" AS "H" ON
--"P"."BatchId" = "H"."BatchId"
--LEFT JOIN "DBInvOne"."ProcessStatus" AS "S" ON
--"S"."ID" = "P"."StatusId"
--WHERE "P"."DocEntry" = (SELECT "DocEntry" FROm OINV WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar))
--AND "P"."DocType" = 13
--AND "P"."CompanyId" = (SELECT "BPLId" FROm OINV WHERE "U_NumTransf" = CAST(T0."DocNum" as nvarchar))) = 4

GROUP BY 
T1."BaseRef",
T1."ItemCode",
T1."FromWhsCod" ,
T1."WhsCode" ,
T0."DocNum",
T0."DocEntry"

ORDER BY 1,2
;



