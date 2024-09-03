--//*********************************************VENDA******************************************************
--Nota de Saída
SELECT
'Invoices' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM OINV T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN INV12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--Dev.Nota de Saída
SELECT
'CreditNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM ORIN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN RIN12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--Entrega
SELECT
'DeliveryNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM ODLN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN DLN12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--Devolução
SELECT
'Returns' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM ORDN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN RDN12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--*********************************************COMPRA******************************************************
--Nota de Entrada
SELECT
'PurchaseInvoices' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM OPCH T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN PCH12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--Dev. Nota de Entrada
SELECT
'PurchaseCreditNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM ORPC T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN RPC12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--Recebimento Mercadorias
SELECT
'PurchaseDeliveryNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM OPDN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN PDN12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N'

UNION ALL

--Dev.Mercadoria
SELECT
'PurchaseReturns' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe",
T2."Description"
FROM ORPD T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessStatus" T2 ON T1."StatusId" = T2."ID"
INNER JOIN RPD12 T3 ON T3."DocEntry" = T0."DocEntry"
WHERE T2."ID" = '25'
AND T0."CANCELED" = 'N';

'