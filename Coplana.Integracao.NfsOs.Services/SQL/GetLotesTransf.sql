/* Nota de Transf. Filiais */
SELECT *
FROM K33P_TRANS_NF_LOTE_ENTRADA  x
WHERE
	x."BaseType" = 13
AND x."DocEntry" = {0}-- (select top 1 "DocEntry" from OINV WHERE "Serial" = {0} and "BPLId" = 6)
AND x."ItemCode" = '{1}'


