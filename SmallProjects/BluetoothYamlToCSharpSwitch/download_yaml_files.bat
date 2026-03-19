REM Downloads the YAML files needed to update our C Sharp program files

curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/characteristic_uuids.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/units.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml