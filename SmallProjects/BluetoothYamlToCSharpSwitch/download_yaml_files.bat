REM Downloads the YAML files needed to update our C Sharp program files

REM Bluetooth has a ton of specific values at https://bitbucket.org/bluetooth-SIG/public/src/main/dp/properties/
REM And also at https://bitbucket.org/bluetooth-SIG/public/src/main/gss/

REM Change 'public/src.main' to 'public/raw/main' to just get the YAML
REM For example: convert https://bitbucket.org/bluetooth-SIG/public/src/main/gss/org.bluetooth.characteristic.body_sensor_location.yaml
REM                 into https://bitbucket.org/bluetooth-SIG/public/raw/main/gss/org.bluetooth.characteristic.body_sensor_location.yaml



curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/core/ad_types.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/characteristic_uuids.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/units.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/service_uuids.yaml

curl --remote-name --get https://bitbucket.org/bluetooth-SIG/public/raw/main/gss/org.bluetooth.characteristic.body_sensor_location.yaml


REM https://bitbucket.org/bluetooth-SIG/public/raw/gss/org.bluetooth.characteristic.body_sensor_location.yaml
REM https://bitbucket.org/bluetooth-SIG/public/raw/gss/org.bluetooth.characteristic.body_sensor_location.yaml
REM https://bitbucket.org/bluetooth-SIG/public/raw/d6855ad309bd25b87e72ab84b7ee5084b662cb3b/gss/org.bluetooth.characteristic.body_sensor_location.yaml
