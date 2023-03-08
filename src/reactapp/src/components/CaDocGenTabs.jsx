import {
    makeStyles,
    shorthands,
    tokens,
    Tab,
    TabList,
    SelectTabData,
    SelectTabEvent,
} from "@fluentui/react-components";
import * as React from "react";
import { useState } from 'react';

import {
    RocketRegular,
    RocketFilled,
    WindowDevToolsRegular,
    WindowDevToolsFilled,
    bundleIcon,
} from "@fluentui/react-icons";
import { CaDocGenManual } from '../components/CaDocGenManual';
import { CaDocGenAuto } from '../components/CaDocGenAuto';

const AutoGeneration = bundleIcon(RocketFilled, RocketRegular);
const ManualGeneration = bundleIcon(WindowDevToolsFilled, WindowDevToolsRegular);

const useStyles = makeStyles({
    root: {
        alignItems: "flex-start",
        display: "flex",
        flexDirection: "column",
        justifyContent: "flex-start",
        ...shorthands.padding("20px", "0px"),
        rowGap: "20px",
    },
    panels: {
        ...shorthands.padding(0, "10px"),
        "& th": {
            textAlign: "left",
            ...shorthands.padding(0, "30px", 0, 0),
        },
    },
    propsTable: {
        "& td:first-child": {
            fontWeight: tokens.fontWeightSemibold,
        },
        "& td": {
            ...shorthands.padding(0, "30px", 0, 0),
        },
    },
});

export const CaDocGenTabs = () => {
    const styles = useStyles();

    const [selectedValue, setSelectedValue] = useState("autogeneration");

    const onTabSelect = (event: SelectTabEvent, data: SelectTabData) => {
        setSelectedValue(data.value);
    };

    const Arrivals = React.memo(() => (
        <div role="tabpanel" aria-labelledby="Arrivals">
            <table>
                <thead>
                    <th>Origin</th>
                    <th>Gate</th>
                    <th>ETA</th>
                </thead>
                <tbody>
                    <tr>
                        <td>DEN</td>
                        <td>C3</td>
                        <td>12:40 PM</td>
                    </tr>
                    <tr>
                        <td>SMF</td>
                        <td>D1</td>
                        <td>1:18 PM</td>
                    </tr>
                    <tr>
                        <td>SFO</td>
                        <td>E18</td>
                        <td>1:42 PM</td>
                    </tr>
                </tbody>
            </table>
        </div>
    ));

    return (
        <div className={styles.root}>
            <TabList selectedValue={selectedValue} onTabSelect={onTabSelect}>
                <Tab id="Automatic" icon={<AutoGeneration />} value="autogeneration">
                    Automatic Generation
                </Tab>
                <Tab id="Departures" icon={<ManualGeneration />} value="manualgeneration">
                    Manual Generation
                </Tab>
            </TabList>
            <div className={styles.panels}>
                {selectedValue === "autogeneration" && <CaDocGenAuto />}
                {selectedValue === "manualgeneration" && <CaDocGenManual />}
            </div>
        </div>
    );
};