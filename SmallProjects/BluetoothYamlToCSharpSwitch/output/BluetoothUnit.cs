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
				case 0x2700: return "System.Func`1[System.String]"; // org.bluetooth.unit.unitless
				case 0x2701: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.metre
				case 0x2702: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass.kilogram
				case 0x2703: return "System.Func`1[System.String]"; // org.bluetooth.unit.time.second
				case 0x2704: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_current.ampere
				case 0x2705: return "System.Func`1[System.String]"; // org.bluetooth.unit.thermodynamic_temperature.kelvin
				case 0x2706: return "System.Func`1[System.String]"; // org.bluetooth.unit.amount_of_substance.mole
				case 0x2707: return "System.Func`1[System.String]"; // org.bluetooth.unit.luminous_intensity.candela
				case 0x2710: return "System.Func`1[System.String]"; // org.bluetooth.unit.area.square_metres
				case 0x2711: return "System.Func`1[System.String]"; // org.bluetooth.unit.volume.cubic_metres
				case 0x2712: return "System.Func`1[System.String]"; // org.bluetooth.unit.velocity.metres_per_second
				case 0x2713: return "System.Func`1[System.String]"; // org.bluetooth.unit.acceleration.metres_per_second_squared
				case 0x2714: return "System.Func`1[System.String]"; // org.bluetooth.unit.wavenumber.reciprocal_metre
				case 0x2715: return "System.Func`1[System.String]"; // org.bluetooth.unit.density.kilogram_per_cubic_metre
				case 0x2716: return "System.Func`1[System.String]"; // org.bluetooth.unit.surface_density.kilogram_per_square_metre
				case 0x2717: return "System.Func`1[System.String]"; // org.bluetooth.unit.specific_volume.cubic_metre_per_kilogram
				case 0x2718: return "System.Func`1[System.String]"; // org.bluetooth.unit.current_density.ampere_per_square_metre
				case 0x2719: return "System.Func`1[System.String]"; // org.bluetooth.unit.magnetic_field_strength.ampere_per_metre
				case 0x271A: return "System.Func`1[System.String]"; // org.bluetooth.unit.amount_concentration.mole_per_cubic_metre
				case 0x271B: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass_concentration.kilogram_per_cubic_metre
				case 0x271C: return "System.Func`1[System.String]"; // org.bluetooth.unit.luminance.candela_per_square_metre
				case 0x271D: return "System.Func`1[System.String]"; // org.bluetooth.unit.refractive_index
				case 0x271E: return "System.Func`1[System.String]"; // org.bluetooth.unit.relative_permeability
				case 0x2720: return "System.Func`1[System.String]"; // org.bluetooth.unit.plane_angle.radian
				case 0x2721: return "System.Func`1[System.String]"; // org.bluetooth.unit.solid_angle.steradian
				case 0x2722: return "System.Func`1[System.String]"; // org.bluetooth.unit.frequency.hertz
				case 0x2723: return "System.Func`1[System.String]"; // org.bluetooth.unit.force.newton
				case 0x2724: return "System.Func`1[System.String]"; // org.bluetooth.unit.pressure.pascal
				case 0x2725: return "System.Func`1[System.String]"; // org.bluetooth.unit.energy.joule
				case 0x2726: return "System.Func`1[System.String]"; // org.bluetooth.unit.power.watt
				case 0x2727: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_charge.coulomb
				case 0x2728: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_potential_difference.volt
				case 0x2729: return "System.Func`1[System.String]"; // org.bluetooth.unit.capacitance.farad
				case 0x272A: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_resistance.ohm
				case 0x272B: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_conductance.siemens
				case 0x272C: return "System.Func`1[System.String]"; // org.bluetooth.unit.magnetic_flux.weber
				case 0x272D: return "System.Func`1[System.String]"; // org.bluetooth.unit.magnetic_flux_density.tesla
				case 0x272E: return "System.Func`1[System.String]"; // org.bluetooth.unit.inductance.henry
				case 0x272F: return "System.Func`1[System.String]"; // org.bluetooth.unit.thermodynamic_temperature.degree_celsius
				case 0x2730: return "System.Func`1[System.String]"; // org.bluetooth.unit.luminous_flux.lumen
				case 0x2731: return "System.Func`1[System.String]"; // org.bluetooth.unit.illuminance.lux
				case 0x2732: return "System.Func`1[System.String]"; // org.bluetooth.unit.activity_referred_to_a_radionuclide.becquerel
				case 0x2733: return "System.Func`1[System.String]"; // org.bluetooth.unit.absorbed_dose.gray
				case 0x2734: return "System.Func`1[System.String]"; // org.bluetooth.unit.dose_equivalent.sievert
				case 0x2735: return "System.Func`1[System.String]"; // org.bluetooth.unit.catalytic_activity.katal
				case 0x2740: return "System.Func`1[System.String]"; // org.bluetooth.unit.dynamic_viscosity.pascal_second
				case 0x2741: return "System.Func`1[System.String]"; // org.bluetooth.unit.moment_of_force.newton_metre
				case 0x2742: return "System.Func`1[System.String]"; // org.bluetooth.unit.surface_tension.newton_per_metre
				case 0x2743: return "System.Func`1[System.String]"; // org.bluetooth.unit.angular_velocity.radian_per_second
				case 0x2744: return "System.Func`1[System.String]"; // org.bluetooth.unit.angular_acceleration.radian_per_second_squared
				case 0x2745: return "System.Func`1[System.String]"; // org.bluetooth.unit.heat_flux_density.watt_per_square_metre
				case 0x2746: return "System.Func`1[System.String]"; // org.bluetooth.unit.heat_capacity.joule_per_kelvin
				case 0x2747: return "System.Func`1[System.String]"; // org.bluetooth.unit.specific_heat_capacity.joule_per_kilogram_kelvin
				case 0x2748: return "System.Func`1[System.String]"; // org.bluetooth.unit.specific_energy.joule_per_kilogram
				case 0x2749: return "System.Func`1[System.String]"; // org.bluetooth.unit.thermal_conductivity.watt_per_metre_kelvin
				case 0x274A: return "System.Func`1[System.String]"; // org.bluetooth.unit.energy_density.joule_per_cubic_metre
				case 0x274B: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_field_strength.volt_per_metre
				case 0x274C: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_charge_density.coulomb_per_cubic_metre
				case 0x274D: return "System.Func`1[System.String]"; // org.bluetooth.unit.surface_charge_density.coulomb_per_square_metre
				case 0x274E: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_flux_density.coulomb_per_square_metre
				case 0x274F: return "System.Func`1[System.String]"; // org.bluetooth.unit.permittivity.farad_per_metre
				case 0x2750: return "System.Func`1[System.String]"; // org.bluetooth.unit.permeability.henry_per_metre
				case 0x2751: return "System.Func`1[System.String]"; // org.bluetooth.unit.molar_energy.joule_per_mole
				case 0x2752: return "System.Func`1[System.String]"; // org.bluetooth.unit.molar_entropy.joule_per_mole_kelvin
				case 0x2753: return "System.Func`1[System.String]"; // org.bluetooth.unit.exposure.coulomb_per_kilogram
				case 0x2754: return "System.Func`1[System.String]"; // org.bluetooth.unit.absorbed_dose_rate.gray_per_second
				case 0x2755: return "System.Func`1[System.String]"; // org.bluetooth.unit.radiant_intensity.watt_per_steradian
				case 0x2756: return "System.Func`1[System.String]"; // org.bluetooth.unit.radiance.watt_per_square_metre_steradian
				case 0x2757: return "System.Func`1[System.String]"; // org.bluetooth.unit.catalytic_activity_concentration.katal_per_cubic_metre
				case 0x2760: return "System.Func`1[System.String]"; // org.bluetooth.unit.time.minute
				case 0x2761: return "System.Func`1[System.String]"; // org.bluetooth.unit.time.hour
				case 0x2762: return "System.Func`1[System.String]"; // org.bluetooth.unit.time.day
				case 0x2763: return "System.Func`1[System.String]"; // org.bluetooth.unit.plane_angle.degree
				case 0x2764: return "System.Func`1[System.String]"; // org.bluetooth.unit.plane_angle.minute
				case 0x2765: return "System.Func`1[System.String]"; // org.bluetooth.unit.plane_angle.second
				case 0x2766: return "System.Func`1[System.String]"; // org.bluetooth.unit.area.hectare
				case 0x2767: return "System.Func`1[System.String]"; // org.bluetooth.unit.volume.litre
				case 0x2768: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass.tonne
				case 0x2780: return "System.Func`1[System.String]"; // org.bluetooth.unit.pressure.bar
				case 0x2781: return "System.Func`1[System.String]"; // org.bluetooth.unit.pressure.millimetre_of_mercury
				case 0x2782: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.†ngstr”m
				case 0x2783: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.nautical_mile
				case 0x2784: return "System.Func`1[System.String]"; // org.bluetooth.unit.area.barn
				case 0x2785: return "System.Func`1[System.String]"; // org.bluetooth.unit.velocity.knot
				case 0x2786: return "System.Func`1[System.String]"; // org.bluetooth.unit.logarithmic_radio_quantity.neper
				case 0x2787: return "System.Func`1[System.String]"; // org.bluetooth.unit.logarithmic_radio_quantity.bel
				case 0x27A0: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.yard
				case 0x27A1: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.parsec
				case 0x27A2: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.inch
				case 0x27A3: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.foot
				case 0x27A4: return "System.Func`1[System.String]"; // org.bluetooth.unit.length.mile
				case 0x27A5: return "System.Func`1[System.String]"; // org.bluetooth.unit.pressure.pound_force_per_square_inch
				case 0x27A6: return "System.Func`1[System.String]"; // org.bluetooth.unit.velocity.kilometre_per_hour
				case 0x27A7: return "System.Func`1[System.String]"; // org.bluetooth.unit.velocity.mile_per_hour
				case 0x27A8: return "System.Func`1[System.String]"; // org.bluetooth.unit.angular_velocity.revolution_per_minute
				case 0x27A9: return "System.Func`1[System.String]"; // org.bluetooth.unit.energy.gram_calorie
				case 0x27AA: return "System.Func`1[System.String]"; // org.bluetooth.unit.energy.kilogram_calorie
				case 0x27AB: return "System.Func`1[System.String]"; // org.bluetooth.unit.energy.kilowatt_hour
				case 0x27AC: return "System.Func`1[System.String]"; // org.bluetooth.unit.thermodynamic_temperature.degree_fahrenheit
				case 0x27AD: return "System.Func`1[System.String]"; // org.bluetooth.unit.percentage
				case 0x27AE: return "System.Func`1[System.String]"; // org.bluetooth.unit.per_mille
				case 0x27AF: return "System.Func`1[System.String]"; // org.bluetooth.unit.period.beats_per_minute
				case 0x27B0: return "System.Func`1[System.String]"; // org.bluetooth.unit.electric_charge.ampere_hours
				case 0x27B1: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass_density.milligram_per_decilitre
				case 0x27B2: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass_density.millimole_per_litre
				case 0x27B3: return "System.Func`1[System.String]"; // org.bluetooth.unit.time.year
				case 0x27B4: return "System.Func`1[System.String]"; // org.bluetooth.unit.time.month
				case 0x27B5: return "System.Func`1[System.String]"; // org.bluetooth.unit.concentration.count_per_cubic_metre
				case 0x27B6: return "System.Func`1[System.String]"; // org.bluetooth.unit.irradiance.watt_per_square_metre
				case 0x27B7: return "System.Func`1[System.String]"; // org.bluetooth.unit.transfer_rate.milliliter_per_kilogram_per_minute
				case 0x27B8: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass.pound
				case 0x27B9: return "System.Func`1[System.String]"; // org.bluetooth.unit.metabolic_equivalent
				case 0x27BA: return "System.Func`1[System.String]"; // org.bluetooth.unit.step_per_minute
				case 0x27BC: return "System.Func`1[System.String]"; // org.bluetooth.unit.stroke_per_minute
				case 0x27BD: return "System.Func`1[System.String]"; // org.bluetooth.unit.velocity.kilometer_per_minute
				case 0x27BE: return "System.Func`1[System.String]"; // org.bluetooth.unit.luminous_efficacy.lumen_per_watt
				case 0x27BF: return "System.Func`1[System.String]"; // org.bluetooth.unit.luminous_energy.lumen_hour
				case 0x27C0: return "System.Func`1[System.String]"; // org.bluetooth.unit.luminous_exposure.lux_hour
				case 0x27C1: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass_flow.gram_per_second
				case 0x27C2: return "System.Func`1[System.String]"; // org.bluetooth.unit.volume_flow.litre_per_second
				case 0x27C3: return "System.Func`1[System.String]"; // org.bluetooth.unit.sound_pressure.decibel_spl
				case 0x27C4: return "System.Func`1[System.String]"; // org.bluetooth.unit.ppm
				case 0x27C5: return "System.Func`1[System.String]"; // org.bluetooth.unit.ppb
				case 0x27C6: return "System.Func`1[System.String]"; // org.bluetooth.unit.mass_density_rate.milligram_per_decilitre_per_minute
				case 0x27C7: return "System.Func`1[System.String]"; // org.bluetooth.unit.energy.kilovolt_ampere_hour
				case 0x27C8: return "System.Func`1[System.String]"; // org.bluetooth.unit.power.volt_ampere
				case 0x27C9: return "System.Func`1[System.String]"; // org.bluetooth.unit.power.gravity_gn
                // endupdatefile:
            }
            return $"{value:X2}";
        }
    }
}
