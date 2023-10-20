OLD_TEXT='мясо'
declare -A REPLACE_TEXT=( ['11.txt']='красная икра' ['12.txt']='черная икра' ['13.txt']='баклажанная')
for KEY in "${!REPLACE_TEXT[@]}"; do
FILE=$KEY
NEW_TEXT="${REPLACE_TEXT[$KEY]}"
sed -i "s/${OLD_TEXT}/${NEW_TEXT}/g" $FILE; 
done

