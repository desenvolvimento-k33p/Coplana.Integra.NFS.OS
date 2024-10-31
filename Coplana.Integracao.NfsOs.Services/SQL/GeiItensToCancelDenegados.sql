--//*********************************************VENDA******************************************************
--Nota de Saída
SELECT
'Invoices' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM OINV T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--Dev.Nota de Saída
SELECT
'CreditNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM ORIN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--Entrega
SELECT
'DeliveryNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM ODLN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--Devolução
SELECT
'Returns' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM ORDN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--*********************************************COMPRA******************************************************
--Nota de Entrada
SELECT
'PurchaseInvoices' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM OPCH T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--Dev. Nota de Entrada
SELECT
'PurchaseCreditNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM ORPC T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--Recebimento Mercadorias
SELECT
'PurchaseDeliveryNotes' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM OPDN T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N'

UNION ALL

--Dev.Mercadoria
SELECT
'PurchaseReturns' as "Entity",
T0."CardCode",
T0."CardName",
T0."DocEntry",
T1."KeyNfe"

FROM ORPD T0 
INNER JOIN "DBInvOne"."Process" T1 ON T0."DocEntry" = T1."DocEntry" AND T0."ObjType" = T1."DocType"
INNER JOIN "DBInvOne"."ProcessHist" T2 ON T2."BatchId" =  T1."BatchId"
WHERE T2."ReturnId" = '302'
AND T0."CANCELED"=  'N';

