# Transfer factor override

The transfer factor is represented as two positive numeric values (`from:to`). The transfer screen displays the applicable factor when both programs are selected and allows the user to override either side.

Accepted examples include `1:1`, `2:1`, `1:2`, and `1.5:2`. Both values must be finite and greater than zero. Frontend validation prevents invalid submissions; the backend validates the values again and uses the supplied ratio for transfer calculations.
