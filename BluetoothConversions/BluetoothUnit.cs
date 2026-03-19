using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothConversions
{
    // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
    // the items marked GATT Unit
    // 2026: or look in the YAML file: https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/units.yaml
    static class BluetoothUnit
    {
        public static string Decode(UInt16 value)
        {
            switch (value)
            {
                // updatefile:
                // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/units.yaml
                // file:units.yaml
                // startupdatefile:
                case 0x2700: return "unitless"; // org.bluetooth.unit.unitless
                case 0x2701: return "length (metre)"; // org.bluetooth.unit.length.metre
                case 0x2702: return "mass (kilogram)"; // org.bluetooth.unit.mass.kilogram
                case 0x2703: return "time (second)"; // org.bluetooth.unit.time.second
                case 0x2704: return "electric current (ampere)"; // org.bluetooth.unit.electric_current.ampere
                case 0x2705: return "thermodynamic temperature (kelvin)"; // org.bluetooth.unit.thermodynamic_temperature.kelvin
                case 0x2706: return "amount of substance (mole)"; // org.bluetooth.unit.amount_of_substance.mole
                case 0x2707: return "luminous intensity (candela)"; // org.bluetooth.unit.luminous_intensity.candela
                case 0x2710: return "area (square metres)"; // org.bluetooth.unit.area.square_metres
                case 0x2711: return "volume (cubic metres)"; // org.bluetooth.unit.volume.cubic_metres
                case 0x2712: return "velocity (metres per second)"; // org.bluetooth.unit.velocity.metres_per_second
                case 0x2713: return "acceleration (metres per second squared)"; // org.bluetooth.unit.acceleration.metres_per_second_squared
                case 0x2714: return "wavenumber (reciprocal metre)"; // org.bluetooth.unit.wavenumber.reciprocal_metre
                case 0x2715: return "density (kilogram per cubic metre)"; // org.bluetooth.unit.density.kilogram_per_cubic_metre
                case 0x2716: return "surface density (kilogram per square metre)"; // org.bluetooth.unit.surface_density.kilogram_per_square_metre
                case 0x2717: return "specific volume (cubic metre per kilogram)"; // org.bluetooth.unit.specific_volume.cubic_metre_per_kilogram
                case 0x2718: return "current density (ampere per square metre)"; // org.bluetooth.unit.current_density.ampere_per_square_metre
                case 0x2719: return "magnetic field strength (ampere per metre)"; // org.bluetooth.unit.magnetic_field_strength.ampere_per_metre
                case 0x271A: return "amount concentration (mole per cubic metre)"; // org.bluetooth.unit.amount_concentration.mole_per_cubic_metre
                case 0x271B: return "mass concentration (kilogram per cubic metre)"; // org.bluetooth.unit.mass_concentration.kilogram_per_cubic_metre
                case 0x271C: return "luminance (candela per square metre)"; // org.bluetooth.unit.luminance.candela_per_square_metre
                case 0x271D: return "refractive index"; // org.bluetooth.unit.refractive_index
                case 0x271E: return "relative permeability"; // org.bluetooth.unit.relative_permeability
                case 0x2720: return "plane angle (radian)"; // org.bluetooth.unit.plane_angle.radian
                case 0x2721: return "solid angle (steradian)"; // org.bluetooth.unit.solid_angle.steradian
                case 0x2722: return "frequency (hertz)"; // org.bluetooth.unit.frequency.hertz
                case 0x2723: return "force (newton)"; // org.bluetooth.unit.force.newton
                case 0x2724: return "pressure (pascal)"; // org.bluetooth.unit.pressure.pascal
                case 0x2725: return "energy (joule)"; // org.bluetooth.unit.energy.joule
                case 0x2726: return "power (watt)"; // org.bluetooth.unit.power.watt
                case 0x2727: return "electric charge (coulomb)"; // org.bluetooth.unit.electric_charge.coulomb
                case 0x2728: return "electric potential difference (volt)"; // org.bluetooth.unit.electric_potential_difference.volt
                case 0x2729: return "capacitance (farad)"; // org.bluetooth.unit.capacitance.farad
                case 0x272A: return "electric resistance (ohm)"; // org.bluetooth.unit.electric_resistance.ohm
                case 0x272B: return "electric conductance (siemens)"; // org.bluetooth.unit.electric_conductance.siemens
                case 0x272C: return "magnetic flux (weber)"; // org.bluetooth.unit.magnetic_flux.weber
                case 0x272D: return "magnetic flux density (tesla)"; // org.bluetooth.unit.magnetic_flux_density.tesla
                case 0x272E: return "inductance (henry)"; // org.bluetooth.unit.inductance.henry
                case 0x272F: return "Celsius temperature (degree Celsius)"; // org.bluetooth.unit.thermodynamic_temperature.degree_celsius
                case 0x2730: return "luminous flux (lumen)"; // org.bluetooth.unit.luminous_flux.lumen
                case 0x2731: return "illuminance (lux)"; // org.bluetooth.unit.illuminance.lux
                case 0x2732: return "activity referred to a radionuclide (becquerel)"; // org.bluetooth.unit.activity_referred_to_a_radionuclide.becquerel
                case 0x2733: return "absorbed dose (gray)"; // org.bluetooth.unit.absorbed_dose.gray
                case 0x2734: return "dose equivalent (sievert)"; // org.bluetooth.unit.dose_equivalent.sievert
                case 0x2735: return "catalytic activity (katal)"; // org.bluetooth.unit.catalytic_activity.katal
                case 0x2740: return "dynamic viscosity (pascal second)"; // org.bluetooth.unit.dynamic_viscosity.pascal_second
                case 0x2741: return "moment of force (newton metre)"; // org.bluetooth.unit.moment_of_force.newton_metre
                case 0x2742: return "surface tension (newton per metre)"; // org.bluetooth.unit.surface_tension.newton_per_metre
                case 0x2743: return "angular velocity (radian per second)"; // org.bluetooth.unit.angular_velocity.radian_per_second
                case 0x2744: return "angular acceleration (radian per second squared)"; // org.bluetooth.unit.angular_acceleration.radian_per_second_squared
                case 0x2745: return "heat flux density (watt per square metre)"; // org.bluetooth.unit.heat_flux_density.watt_per_square_metre
                case 0x2746: return "heat capacity (joule per kelvin)"; // org.bluetooth.unit.heat_capacity.joule_per_kelvin
                case 0x2747: return "specific heat capacity (joule per kilogram kelvin)"; // org.bluetooth.unit.specific_heat_capacity.joule_per_kilogram_kelvin
                case 0x2748: return "specific energy (joule per kilogram)"; // org.bluetooth.unit.specific_energy.joule_per_kilogram
                case 0x2749: return "thermal conductivity (watt per metre kelvin)"; // org.bluetooth.unit.thermal_conductivity.watt_per_metre_kelvin
                case 0x274A: return "energy density (joule per cubic metre)"; // org.bluetooth.unit.energy_density.joule_per_cubic_metre
                case 0x274B: return "electric field strength (volt per metre)"; // org.bluetooth.unit.electric_field_strength.volt_per_metre
                case 0x274C: return "electric charge density (coulomb per cubic metre)"; // org.bluetooth.unit.electric_charge_density.coulomb_per_cubic_metre
                case 0x274D: return "surface charge density (coulomb per square metre)"; // org.bluetooth.unit.surface_charge_density.coulomb_per_square_metre
                case 0x274E: return "electric flux density (coulomb per square metre)"; // org.bluetooth.unit.electric_flux_density.coulomb_per_square_metre
                case 0x274F: return "permittivity (farad per metre)"; // org.bluetooth.unit.permittivity.farad_per_metre
                case 0x2750: return "permeability (henry per metre)"; // org.bluetooth.unit.permeability.henry_per_metre
                case 0x2751: return "molar energy (joule per mole)"; // org.bluetooth.unit.molar_energy.joule_per_mole
                case 0x2752: return "molar entropy (joule per mole kelvin)"; // org.bluetooth.unit.molar_entropy.joule_per_mole_kelvin
                case 0x2753: return "exposure (coulomb per kilogram)"; // org.bluetooth.unit.exposure.coulomb_per_kilogram
                case 0x2754: return "absorbed dose rate (gray per second)"; // org.bluetooth.unit.absorbed_dose_rate.gray_per_second
                case 0x2755: return "radiant intensity (watt per steradian)"; // org.bluetooth.unit.radiant_intensity.watt_per_steradian
                case 0x2756: return "radiance (watt per square metre steradian)"; // org.bluetooth.unit.radiance.watt_per_square_metre_steradian
                case 0x2757: return "catalytic activity concentration (katal per cubic metre)"; // org.bluetooth.unit.catalytic_activity_concentration.katal_per_cubic_metre
                case 0x2760: return "time (minute)"; // org.bluetooth.unit.time.minute
                case 0x2761: return "time (hour)"; // org.bluetooth.unit.time.hour
                case 0x2762: return "time (day)"; // org.bluetooth.unit.time.day
                case 0x2763: return "plane angle (degree)"; // org.bluetooth.unit.plane_angle.degree
                case 0x2764: return "plane angle (minute)"; // org.bluetooth.unit.plane_angle.minute
                case 0x2765: return "plane angle (second)"; // org.bluetooth.unit.plane_angle.second
                case 0x2766: return "area (hectare)"; // org.bluetooth.unit.area.hectare
                case 0x2767: return "volume (litre)"; // org.bluetooth.unit.volume.litre
                case 0x2768: return "mass (tonne)"; // org.bluetooth.unit.mass.tonne
                case 0x2780: return "pressure (bar)"; // org.bluetooth.unit.pressure.bar
                case 0x2781: return "pressure (millimetre of mercury)"; // org.bluetooth.unit.pressure.millimetre_of_mercury
                case 0x2782: return "length ( ngstr m)"; // org.bluetooth.unit.length. ngstr m
                case 0x2783: return "length (nautical mile)"; // org.bluetooth.unit.length.nautical_mile
                case 0x2784: return "area (barn)"; // org.bluetooth.unit.area.barn
                case 0x2785: return "velocity (knot)"; // org.bluetooth.unit.velocity.knot
                case 0x2786: return "logarithmic radio quantity (neper)"; // org.bluetooth.unit.logarithmic_radio_quantity.neper
                case 0x2787: return "logarithmic radio quantity (bel)"; // org.bluetooth.unit.logarithmic_radio_quantity.bel
                case 0x27A0: return "length (yard)"; // org.bluetooth.unit.length.yard
                case 0x27A1: return "length (parsec)"; // org.bluetooth.unit.length.parsec
                case 0x27A2: return "length (inch)"; // org.bluetooth.unit.length.inch
                case 0x27A3: return "length (foot)"; // org.bluetooth.unit.length.foot
                case 0x27A4: return "length (mile)"; // org.bluetooth.unit.length.mile
                case 0x27A5: return "pressure (pound-force per square inch)"; // org.bluetooth.unit.pressure.pound_force_per_square_inch
                case 0x27A6: return "velocity (kilometre per hour)"; // org.bluetooth.unit.velocity.kilometre_per_hour
                case 0x27A7: return "velocity (mile per hour)"; // org.bluetooth.unit.velocity.mile_per_hour
                case 0x27A8: return "angular velocity (revolution per minute)"; // org.bluetooth.unit.angular_velocity.revolution_per_minute
                case 0x27A9: return "energy (gram calorie)"; // org.bluetooth.unit.energy.gram_calorie
                case 0x27AA: return "energy (kilogram calorie)"; // org.bluetooth.unit.energy.kilogram_calorie
                case 0x27AB: return "energy (kilowatt hour)"; // org.bluetooth.unit.energy.kilowatt_hour
                case 0x27AC: return "thermodynamic temperature (degree Fahrenheit)"; // org.bluetooth.unit.thermodynamic_temperature.degree_fahrenheit
                case 0x27AD: return "percentage"; // org.bluetooth.unit.percentage
                case 0x27AE: return "per mille"; // org.bluetooth.unit.per_mille
                case 0x27AF: return "period (beats per minute)"; // org.bluetooth.unit.period.beats_per_minute
                case 0x27B0: return "electric charge (ampere hours)"; // org.bluetooth.unit.electric_charge.ampere_hours
                case 0x27B1: return "mass density (milligram per decilitre)"; // org.bluetooth.unit.mass_density.milligram_per_decilitre
                case 0x27B2: return "mass density (millimole per litre)"; // org.bluetooth.unit.mass_density.millimole_per_litre
                case 0x27B3: return "time (year)"; // org.bluetooth.unit.time.year
                case 0x27B4: return "time (month)"; // org.bluetooth.unit.time.month
                case 0x27B5: return "concentration (count per cubic metre)"; // org.bluetooth.unit.concentration.count_per_cubic_metre
                case 0x27B6: return "irradiance (watt per square metre)"; // org.bluetooth.unit.irradiance.watt_per_square_metre
                case 0x27B7: return "milliliter (per kilogram per minute)"; // org.bluetooth.unit.transfer_rate.milliliter_per_kilogram_per_minute
                case 0x27B8: return "mass (pound)"; // org.bluetooth.unit.mass.pound
                case 0x27B9: return "metabolic equivalent"; // org.bluetooth.unit.metabolic_equivalent
                case 0x27BA: return "step (per minute)"; // org.bluetooth.unit.step_per_minute
                case 0x27BC: return "stroke (per minute)"; // org.bluetooth.unit.stroke_per_minute
                case 0x27BD: return "pace (kilometre per minute)"; // org.bluetooth.unit.velocity.kilometer_per_minute
                case 0x27BE: return "luminous efficacy (lumen per watt)"; // org.bluetooth.unit.luminous_efficacy.lumen_per_watt
                case 0x27BF: return "luminous energy (lumen hour)"; // org.bluetooth.unit.luminous_energy.lumen_hour
                case 0x27C0: return "luminous exposure (lux hour)"; // org.bluetooth.unit.luminous_exposure.lux_hour
                case 0x27C1: return "mass flow (gram per second)"; // org.bluetooth.unit.mass_flow.gram_per_second
                case 0x27C2: return "volume flow (litre per second)"; // org.bluetooth.unit.volume_flow.litre_per_second
                case 0x27C3: return "sound pressure (decibel)"; // org.bluetooth.unit.sound_pressure.decibel_spl
                case 0x27C4: return "parts per million"; // org.bluetooth.unit.ppm
                case 0x27C5: return "parts per billion"; // org.bluetooth.unit.ppb
                case 0x27C6: return "mass density rate ((milligram per decilitre) per minute)"; // org.bluetooth.unit.mass_density_rate.milligram_per_decilitre_per_minute
                case 0x27C7: return "Electrical Apparent Energy (kilovolt ampere hour)"; // org.bluetooth.unit.energy.kilovolt_ampere_hour
                case 0x27C8: return "Electrical Apparent Power (volt ampere)"; // org.bluetooth.unit.power.volt_ampere
                case 0x27C9: return "Gravity (g\textsubscript{n})"; // org.bluetooth.unit.power.gravity_gn

                // endupdatefile:
            }
            return $"{value:X2}";
        }
    }
}
