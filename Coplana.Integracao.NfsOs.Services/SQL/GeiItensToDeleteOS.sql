SELECT
OINV."U_CodOrigemAgri" AS "DocEntry",
 --(ROW_NUMBER() OVER(PARTITION BY OINV."U_CodOrigemAgri" ORDER BY INV1."LineNum")) + IFNULL((SELECT MAX(AO."LineId") FROM "@AGRM_OSOA" AO WHERE AO."DocEntry" = OINV."U_CodOrigemAgri" ),0) AS "LineId"  ,
 --(ROW_NUMBER() OVER(PARTITION BY OINV."U_CodOrigemAgri" ORDER BY INV1."LineNum")) + IFNULL((SELECT MAX(AO."LineId") FROM "@AGRM_OSOA" AO WHERE AO."DocEntry" = OINV."U_CodOrigemAgri" ),0) AS "VisOrder"  ,
(SELECT "LineId" FROM "@AGRM_OSOA" WHERE "DocEntry" = OINV."U_CodOrigemAgri" AND "U_NumeroNota" = OINV."Serial" and "U_CodItem" = INV1."ItemCode") as "LineId",
(SELECT "LineId" FROM "@AGRM_OSOA" WHERE "DocEntry" = OINV."U_CodOrigemAgri" AND "U_NumeroNota" = OINV."Serial" and "U_CodItem" = INV1."ItemCode") as "VisOrder",
'D' AS "U_Tipo",
OINV."DocDate" AS "U_DtAplicaca",
OINV."DocDate" AS "U_DtLiberaca",
INV1."ItemCode" AS "U_CodItem",
OITM."ItemName" AS "U_DscItem",
OITM."FirmCode" AS "U_CodigoFabricante",
INV1."Quantity" AS "U_Quantidade",
INV1."Price" AS "U_ValorUnitario",
INV1."WhsCode" AS "U_CodDeposit",
OWHS."WhsName" AS "U_NomDeposit",
OINV."Serial" AS "U_NumeroNota",
INV1."StockPrice" AS "U_Custo"
FROM OINV
INNER JOIN INV1
ON OINV."DocEntry" = INV1."DocEntry"
INNER JOIN OITM
ON INV1."ItemCode" = OITM."ItemCode"
INNER JOIN OWHS
ON INV1."WhsCode" = OWHS."WhsCode"
WHERE 1=1
AND OINV."CANCELED" = 'Y'
AND OINV."U_CodOrigemAgri" IS NOT NULL
AND EXISTS (SELECT * FROM "@AGRM_OSOA" WHERE OINV."Serial" = "@AGRM_OSOA"."U_NumeroNota" )
ORDER BY 1,2