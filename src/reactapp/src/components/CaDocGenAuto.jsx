import * as React from "react";
import { useState, useEffect } from 'react';
import {
    makeStyles,
    tokens,
} from "@fluentui/react-components";

import { Providers, ProviderState } from "@microsoft/mgt";
import { Login } from '@microsoft/mgt-react';
import { CaDocGenButton } from '../components/CaDocGenButton';
import { Alert } from "@fluentui/react-components/unstable";
import { OptionsRegular, } from "@fluentui/react-icons";
import { Accordion, AccordionHeader, AccordionItem, AccordionPanel, Checkbox, useId, Combobox, Option, ComboboxProps, shorthands, typographyStyles } from "@fluentui/react-components";

const useStyles = makeStyles({
    base: {
        display: "flex",
        flexDirection: "column",
        "& > label": {
            display: "block",
            marginBottom: tokens.spacingVerticalMNudge,
        },
    },
    textarea: {
        height: "200px",
    },
    wrapper: {
        marginTop: tokens.spacingVerticalM,
        columnGap: "15px",
        display: "flex",
    },
    settingsForm: {
        // Stack the label above the field with a gap
        display: "grid",
        gridTemplateRows: "repeat(1fr)",
        justifyItems: "start",
        ...shorthands.gap("2px"),
        //maxWidth: "400px",
      },
      description: {
        ...typographyStyles.caption1,
      },
    
});

function useIsSignedIn(): [boolean] {
    const [isSignedIn, setIsSignedIn] = useState(false);

    useEffect(() => {
        const updateState = () => {
            const provider = Providers.globalProvider;
            setIsSignedIn(provider && provider.state === ProviderState.SignedIn);
        };

        Providers.onProviderUpdated(updateState);
        updateState();

        return () => {
            Providers.removeProviderUpdatedListener(updateState);
        }
    }, []);

    return [isSignedIn];
}

export const CaDocGenAuto = (props: Partial<ComboboxProps>) => {
    const styles = useStyles();
    const [isSignedIn] = useIsSignedIn();

    const comboId = useId("combo-multi");
    const selectedListId = `${comboId}-selection`;
    const [selectedOptions, setSelectedOptions] = React.useState([]);
    const options = ["Policy name", "Group", "User", "Application", "External tenant", "Terms of use", "Named location"];
    const onSelect: ComboboxProps["onOptionSelect"] = (event, data) => {
        setSelectedOptions(data.selectedOptions);
      };
    const labelledBy = selectedOptions.length > 0 ? `${comboId} ${selectedListId}` : comboId;
    const [removeSlideGrouping, setRemoveSlideGrouping] = React.useState(false);


    return (
        <>
            <div className={styles.base}>
                <p>Dynamically generate a PowerPoint presentation for the conditional access policies in your tenant.</p>
                <Accordion collapsible={true}>
                    <AccordionItem value="1">
                        <AccordionHeader icon={<OptionsRegular />}>
                            Settings
                        </AccordionHeader>
                        <AccordionPanel>
                            <div className={styles.settingsForm}>
                                <Checkbox
                                    checked={removeSlideGrouping}
                                    onChange={(ev, data) => setRemoveSlideGrouping(data.checked)}
                                    label="Don't group slides by status (enabled, disabled, report only)"
                                    /><br/>
                                <label id={comboId}>Hide PII & confidential information</label>
                                <Combobox aria-labelledby={labelledBy} multiselect={true} placeholder="Select names to hide" onOptionSelect={onSelect} {...props} >
                                    {options.map((option) => (
                                    <Option key={option}>
                                        {option}
                                    </Option>
                                    ))}
                                </Combobox><br/>
                            </div>
                        </AccordionPanel>
                    </AccordionItem>
                </Accordion>

                {isSignedIn &&
                    <>
                        <CaDocGenButton maskOptions={selectedOptions} groupSlidesByState={!removeSlideGrouping}/>
                    </>
                }

                {!isSignedIn &&
                    <div>
                        <Alert intent="info" >
                            <p>Please sign in to generate documentation.</p>
                            <p>
                                <Login />
                            </p>
                        </Alert>
                    </div>
                }
            </div>
        </>
    );
};



