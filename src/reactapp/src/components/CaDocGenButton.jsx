import { makeStyles, tokens, Button } from "@fluentui/react-components";
import * as React from "react";
import { CalendarMonthRegular, } from "@fluentui/react-icons";
import { Providers, ProviderState } from "@microsoft/mgt";

const useStyles = makeStyles({
    wrapper: {
        marginTop: tokens.spacingVerticalM,
        columnGap: "15px",
        display: "flex",
    },
});

export const CaDocGenButton = ({ isManual, caPolicyJson, token }) => {
    const styles = useStyles();

    const handleClick = async () => {

        let policy = {
            conditionalAccessPolicyJson: caPolicyJson,
            isManual: isManual
        };

        let token = "";
        if (Providers.globalProvider.state === ProviderState.SignedIn) {
            token = await Providers.globalProvider.getAccessToken({ scopes: ['User.Read'] })
        }

        const options = {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
                'X-PowerPointGeneration-Token': token
            },
            body: JSON.stringify(policy)
        };


        fetch('/powerpoint', options)
            .then((response) => response.blob())
            .then((blob) => {

                // 2. Create blob link to download
                const url = window.URL.createObjectURL(new Blob([blob]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', `Conditional Access Policy Documentation.pptx`);
                // 3. Append to html page
                document.body.appendChild(link);
                // 4. Force download
                link.click();
                // 5. Clean up and remove the link
                link.parentNode.removeChild(link);

            })
            .catch((error) => {
                error.json().then((json) => {
                    this.setState({
                        errors: json,
                    });
                })
            });
    };

    return (
        <div className={styles.wrapper}>
            <Button appearance="primary" icon={<CalendarMonthRegular />}
                onClick={handleClick}
            >
                Generate documentation
            </Button>
        </div>
    );
};