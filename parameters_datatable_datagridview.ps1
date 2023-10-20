# https://blog.russmax.com/powershell-using-datatables/

##The following code sample creates a data table and adds custom columns to it.##

######################################
##Creating and Returning a DataTable##
######################################
function createDT()
{
    ###Creating a new DataTable###
    $tempTable = New-Object System.Data.DataTable

    ##Creating Columns for DataTable##
    $column1 = New-Object System.Data.DataColumn(“ColumnName1”)
    $column2 = New-Object System.Data.DataColumn(“ColumnName2”)
    $column1 = New-Object system.Data.DataColumn name,([string])
    $column3 = New-Object System.Data.DataColumn(“ColumnName3”)

    ###Adding Columns for DataTable###
    $tempTable.columns.Add($column1)
    $tempTable.columns.Add($column2)
    $tempTable.columns.Add($column3)

    return ,$tempTable
}
